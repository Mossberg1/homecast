using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StreamingApplication.Data;
using StreamingApplication.Interfaces;
using StreamingApplication.Data.Entities;
using StreamingApplication.Helpers;


namespace StreamingApplication.Services;

public class TokenService : ITokenService {
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ApplicationDbContext _dbContext;


    public TokenService(IConfiguration config, ApplicationDbContext dbContext) {
        _secret = config["JwtAuthentication:SecretKey"] ?? throw new NullReferenceException("Secret key is null");
        _issuer = config["JwtAuthentication:Issuer"] ?? throw new NullReferenceException("Issuer is null");
        _audience = config["JwtAuthentication:Audience"] ?? throw new NullReferenceException("Audience is null");
        _dbContext = dbContext;
    }


    /* Method to generate a JWT token for a user. */
    public async Task<string> GenerateTokenAsync(ApplicationUser user) {
        if (string.IsNullOrEmpty(user.UserName)) {
            throw new ArgumentNullException(nameof(user.UserName), "UserName can't be null or empty.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);

        var roles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync();

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    /* Method to generate a refresh token for a user. */
    public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user) {
        var refreshToken = new RefreshToken {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken.Token;
    }
    
    
    /* Method to remove a refresh token from the database. */
    public async Task<bool> RemoveRefreshTokenAsync(string id) {
        var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == id);
        if (refreshToken == null) {
            return false;
        }

        _dbContext.RefreshTokens.Remove(refreshToken);
        return true;
    }
}