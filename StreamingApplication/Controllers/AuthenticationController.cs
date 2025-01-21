using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Forms;
using StreamingApplication.Helpers;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Controllers;

[ApiController]
[Route("api/v1/[controller]/")]
public class AuthenticationController : ControllerBase {
    private readonly IUserService _userService;


    public AuthenticationController(IUserService userService) {
        _userService = userService;
    }


    /* Controller to register a regular user. */
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterForm registerForm) {
        var result = await _userService.CreateUserAsync(registerForm, "Base");
        if (!result.Succeeded) {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "User was registered." });
    }
    
    
    /*
     * Controller to logout current user.
     */
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout() {
        await _userService.LogoutUserAsync();
        return NoContent();
    }


    /* Controller to login a user. */
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginForm loginForm) {
        var tokens = await _userService.LoginUserAsync(loginForm);
        if (tokens == null) {
            return Unauthorized();
        }

        return Ok(tokens);
    }


    /* Controller to generate a new JWT token with a refresh token. */
    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> RefreshToken(string refreshToken) {
        var tokens = await _userService.GenerateNewToken(refreshToken);
        if (tokens == null) {
            return BadRequest("Invalid refresh token.");
        }

        return Ok(tokens);
    }


    /* Controller to register a admin user. */
    [HttpPost("register/admin")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterForm registerForm) {
        var result = await _userService.CreateUserAsync(registerForm, "Admin");
        if (!result.Succeeded) {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "Admin was registered." });
    }


    /* Controller to change password */
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm form) {
        var userId = User.FindFirst("nameid")?.Value;
        if (userId == null) {
            return Unauthorized("Sub claim not found.");
        }

        var res = await _userService.UpdatePasswordAsync(userId, form);
        if (!res) {
            return BadRequest("Failed to update password.");
        }

        return Ok(new { Message = "Password updated." });
    }
}