namespace StreamingApplication.Helpers.Parameters;

public class RequestParameters {
    public string? Query { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; init; } = 24;


    public int CalculateSkip() {
        return (PageNumber - 1) * PageSize;
    }
}