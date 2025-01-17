using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StreamingApplication.Interfaces;

public interface IFileService {
    public long GetSize(string path);
    public bool Validate(IFormFile file);
    public Task<string> SaveAsync(IFormFile file, string uploadDirectory);
}