using System.ComponentModel.DataAnnotations;

namespace StreamingApplication.Data.DTOs.Movie;

public class MovieCreateDTO {
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public int? MediaFileId { get; set; }
}