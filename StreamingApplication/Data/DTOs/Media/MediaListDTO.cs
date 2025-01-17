using System;

namespace StreamingApplication.Data.DTOs.Media;

public class MediaListDTO {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public long Size { get; set; }
    public TimeSpan Duration { get; set; }
}