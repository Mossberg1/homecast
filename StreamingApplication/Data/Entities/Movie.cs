using System.ComponentModel.DataAnnotations;
using StreamingApplication.Data.Base;
using StreamingApplication.Enumerations;


namespace StreamingApplication.Data.Entities;

public class Movie : BaseEntity {
    [Required] public required string Name { get; set; }

    public string? Description { get; set; }
    
    [Required]
    public required Genre Genre { get; set; }

    [Required] public int MediaFileId { get; set; }

    public required Media MediaFile { get; set; }
}