using System.ComponentModel.DataAnnotations;
using StreamingApplication.Enumerations;

namespace StreamingApplication.Data.DTOs.Media;

public class MediaCreateDTO {
    [Required] public required string Path { get; set; }

    [Required] public MediaType Type { get; set; }
}