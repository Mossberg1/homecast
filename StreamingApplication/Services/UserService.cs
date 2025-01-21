using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StreamingApplication.Data;
using StreamingApplication.Interfaces;
using StreamingApplication.Data.Entities;
using StreamingApplication.Forms;
using StreamingApplication.Helpers;
using StreamingApplication.Helpers.Response;


namespace StreamingApplication.Services;


public class UserService : IUserService {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserService> _logger;


    public UserService(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sim, ITokenService ts, IMapper mapper, ApplicationDbContext dbContext, ILogger<UserService> logger) {
        _userManager = um;
        _signInManager = sim;
        _tokenService = ts;
        _mapper = mapper;
        _dbContext = dbContext;
        _logger = logger;
    }


    /*
     * Method to create a user with Base role.
     * If you create a admin user for the first time, the default admin user will be removed.
     */
    public async Task<IdentityResult> CreateUserAsync(RegisterForm registerForm, string role) {
        var user = _mapper.Map<ApplicationUser>(registerForm);

        var res = await _userManager.CreateAsync(user, registerForm.Password);
        await _userManager.AddToRoleAsync(user, role);

        if (!res.Succeeded) {
            _logger.LogError($"Failed to create user: {res.Errors.First().Description}");
        } else {
            _logger.LogInformation($"User created: {user.UserName}");
        }

        if (role == UserRole.Admin) {
            var baseAdmin = await _userManager.FindByNameAsync("admin");
            if (baseAdmin != null) {
                await _userManager.RemoveAuthenticationTokenAsync(baseAdmin, "StreamingApplication", "AccessToken");
                await _tokenService.RemoveRefreshTokensAsync(baseAdmin.Id);
                await _userManager.DeleteAsync(baseAdmin);
                
                _logger.LogInformation("Default admin user removed.");
            }
        }

        return res;
    }


    /* Method to login a user. */
    public async Task<TokenResponse?> LoginUserAsync(LoginForm loginForm) {
        var result = await _signInManager.PasswordSignInAsync(loginForm.UserName, loginForm.Password, false, false);
        if (!result.Succeeded) {
            _logger.LogError($"Failed to login user: {loginForm.UserName}");
            return null;
        }

        var user = await _userManager.FindByNameAsync(loginForm.UserName);
        if (user == null) {
            _logger.LogWarning($"Could not find user: {loginForm.UserName}");
            return null;
        }

        var currentRefreshTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .ToListAsync();

        if (currentRefreshTokens.Any()) {
            _logger.LogInformation($"Removing {currentRefreshTokens.Count} refresh tokens for user: {user.UserName}");
            _dbContext.RefreshTokens.RemoveRange(currentRefreshTokens);
            await _dbContext.SaveChangesAsync();
        }

        var tokenResponse = new TokenResponse {
            AccessToken = await _tokenService.GenerateTokenAsync(user),
            RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user)
        };

        _logger.LogInformation($"Successfully generated tokens for: {user.UserName}");

        return tokenResponse;
    }


    /* Method to generate a new token for a user with a specified refreshToken. */
    public async Task<TokenResponse?> GenerateNewToken(string refreshToken) {
        var rt = await _dbContext.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);
        if (rt == null) {
            _logger.LogWarning("Refresh token not valid.");
            return null;
        }

        if (rt.Expires < DateTime.UtcNow) {
            _logger.LogWarning("Refresh token has expired.");
            _dbContext.RefreshTokens.Remove(rt);
            await _dbContext.SaveChangesAsync();

            return null;
        }

        var user = await _userManager.FindByIdAsync(rt.UserId);
        if (user == null) {
            _logger.LogWarning($"User with id: {rt.UserId} not found.");
            return null;
        }

        _dbContext.RefreshTokens.Remove(rt);
        await _dbContext.SaveChangesAsync();

        var token = await _tokenService.GenerateTokenAsync(user);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        _logger.LogInformation("Successfully generated new tokens.");

        return new TokenResponse { AccessToken = token, RefreshToken = newRefreshToken };
    }


    /* Method to update a user password. */
    public async Task<bool> UpdatePasswordAsync(string id, ChangePasswordForm form) {
        if (form.NewPassword != form.ConfirmPassword) {
            _logger.LogWarning("New password and confirm password does not match.");
            return false;
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) {
            _logger.LogError($"User with id: {id} not found.");
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, form.CurrentPassword, form.NewPassword);
        if (!result.Succeeded) {
            _logger.LogError($"Failed to update password: {result.Errors.First().Description}");
            return false;
        }

        _logger.LogInformation("Password updated successfully.");

        return true;
    }
}