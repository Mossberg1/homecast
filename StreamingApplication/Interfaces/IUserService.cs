using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Forms;
using StreamingApplication.Helpers.Response;

namespace StreamingApplication.Interfaces;

public interface IUserService {
    public Task<IdentityResult> CreateUserAsync(RegisterForm registerForm, string role);
    public Task<TokenResponse?> GenerateNewToken(string refreshToken);
    public Task LogoutUserAsync();
    public Task<TokenResponse?> LoginUserAsync(LoginForm loginForm);
    public Task<bool> UpdatePasswordAsync(string id, ChangePasswordForm form);
}