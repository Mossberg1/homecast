using StreamingApplication.Data.DTOs.Media;

namespace StreamingApplication.Data.DTOs.Movie;

public class MovieDTO {
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required MediaDTO MediaFile { get; set; }
}