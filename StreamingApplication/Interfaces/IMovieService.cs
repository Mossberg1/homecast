using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.DTOs.Movie;
using StreamingApplication.Forms;
using StreamingApplication.Helpers.Parameters;

namespace StreamingApplication.Interfaces;

public interface IMovieService {

    public Task<int> CreateAsync(MovieCreateDTO createDTO);
    public Task<int> CreateAndUploadAsync(MovieUploadDTO movieUploadDTO, MediaUploadDTO mediaUploadDTO, IFormFile file);
    public Task<bool> DeleteAsync(int id);
    public Task<List<MovieListDTO>> GetAllAsync(MovieParameters parameters);
    public Task<MovieDTO?> GetByIdAsync(int id);
    public Task<MovieDTO?> UpdateAsync(int id, MovieUpdateDTO updateDTO);
    
}