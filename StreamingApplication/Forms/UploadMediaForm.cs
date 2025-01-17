using Microsoft.AspNetCore.Http;
using StreamingApplication.Data.DTOs.Media;

namespace StreamingApplication.Forms;

public class UploadMediaForm {
    public required MediaUploadDTO UploadDTO { get; set; }
    public required IFormFile File { get; set; }
}