using System.ComponentModel.DataAnnotations;

namespace StreamingApplication.Data.DTOs.Movie;

public class MovieUploadDTO {
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
}