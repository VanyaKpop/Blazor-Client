using System.ComponentModel.DataAnnotations;

namespace Blazor_Client.Models;

public record TokenRequest(string token);

public class UserRequest
{

    [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
    public string? Username { get; set; }

    public string? Password { get; set; }
}

public class TestRequest
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public required string DataJson { get; set; }
}