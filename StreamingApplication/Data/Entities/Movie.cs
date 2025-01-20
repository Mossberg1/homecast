using System.ComponentModel.DataAnnotations;
using StreamingApplication.Data.Base;
using StreamingApplication.Enumerations;


namespace StreamingApplication.Data.Entities;

public class Movie : BaseEntity {
    
    [Required]
    [MaxLength(64)]
    public required string Name { get; set; }
    
    [MaxLength(512)]
    public string? Description { get; set; }
    
    [Required]
    public required Genre Genre { get; set; }

    [Required] public int MediaFileId { get; set; }

    public required Media MediaFile { get; set; }
}