using System;
using StreamingApplication.Enumerations;

namespace StreamingApplication.Data.DTOs.Media;

public class MediaDTO {
    public int Id { get; set; }
    public required string Path { get; set; }
    public long Size { get; set; }
    public TimeSpan Duration { get; set; }
    public MediaType Type { get; set; }
}