using System.Threading.Tasks;
using StreamingApplication.Data.Entities;

namespace StreamingApplication.Interfaces;

public interface ITokenService {
    public Task<string> GenerateTokenAsync(ApplicationUser user);
    public Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
}