using System;

namespace StreamingApplication.Data.Entities;

public class RefreshToken {
    public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime Expires { get; set; }
}