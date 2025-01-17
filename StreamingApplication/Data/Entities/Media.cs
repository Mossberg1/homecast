using System;
using System.ComponentModel.DataAnnotations;
using StreamingApplication.Data.Base;
using StreamingApplication.Enumerations;


namespace StreamingApplication.Data.Entities;

public class Media : BaseEntity {
    [Required] [MaxLength(64)] public required string Name { get; set; }

    [Required] [MaxLength(256)] public required string Path { get; set; }

    [Required] public long Size { get; set; }

    [Required] public TimeSpan Duration { get; set; }

    [Required] public MediaType Type { get; set; }
}