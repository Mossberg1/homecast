using Microsoft.AspNetCore.Http;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.DTOs.Movie;

namespace StreamingApplication.Forms;

public class UploadMovieForm {
    public required MovieUploadDTO MovieUpload { get; set; }
    public required MediaUploadDTO MediaUpload { get; set; }
    public required IFormFile File { get; set; }
}