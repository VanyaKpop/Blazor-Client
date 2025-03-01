using System.ComponentModel.DataAnnotations;

namespace Blazor_Client.Models;

public record TokenRequest(string token);

public class UserRequest
{

    [StringLength(16, ErrorMessage = "Identifier too long (16 character limit).")]
    public required string Username { get; set; }

    public required string Password { get; set; }
}

public class TestRequest
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public List<Question> Questions { get; set; } = new();
    public List<Answer> Answers { get; set; } = new();
}