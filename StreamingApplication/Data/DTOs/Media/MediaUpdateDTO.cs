using System.ComponentModel.DataAnnotations;

namespace StreamingApplication.Data.DTOs.Media;

public class MediaUpdateDTO {
    [Required] public required string Name { get; set; }
}