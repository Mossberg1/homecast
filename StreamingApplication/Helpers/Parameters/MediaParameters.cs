namespace StreamingApplication.Helpers.Parameters;

public class MediaParameters : RequestParameters {
    public long MinDuration { get; set; } = 0;
    public long MaxDuration { get; set; } = long.MaxValue;
    public long MinSize { get; set; } = 0;
    public long MaxSize { get; set; } = long.MaxValue;
    public string? Type { get; set; }
}