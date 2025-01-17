using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Helpers.Parameters;


namespace StreamingApplication.Interfaces;

public interface IMediaService {
    public Task<int> CreateAsync(MediaCreateDTO createDTO);
    public Task<bool> DeleteAsync(int id);
    public Task<List<MediaListDTO>> GetAllAsync(MediaParameters parameters);
    public Task<MediaDTO?> GetByIdAsync(int id);
    public Task<MediaDTO?> UpdateAsync(int id, MediaUpdateDTO updateDTO);
    public Task<int> UploadAsync(IFormFile file, MediaUploadDTO uploadDTO);
}