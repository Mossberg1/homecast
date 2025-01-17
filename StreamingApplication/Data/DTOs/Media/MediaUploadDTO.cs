using System.ComponentModel.DataAnnotations;
using StreamingApplication.Enumerations;


namespace StreamingApplication.Data.DTOs.Media;

public class MediaUploadDTO {
    [Required] public MediaType MediaType { get; set; }
}