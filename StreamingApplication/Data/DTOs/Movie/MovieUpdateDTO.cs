using System.ComponentModel.DataAnnotations;

namespace StreamingApplication.Data.DTOs.Movie;

public class MovieUpdateDTO {
    public string? Name { get; set; }
    public string? Description { get; set; }
}