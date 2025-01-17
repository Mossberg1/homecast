namespace StreamingApplication.Helpers.Parameters;

public class MovieParameters : RequestParameters {
    public long MinDuration { get; set; } = 0;
    public long MaxDuration { get; set; } = long.MaxValue;
    public string? Genre { get; set; }
}