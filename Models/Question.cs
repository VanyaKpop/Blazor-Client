namespace Blazor_Client.Models;

public class Question
{
    public required string Type { get; set; }
    public required bool IsRequired { get; set; }
    public required string Body { get; set; }
    public string? UserAnswer { get; set; } = null;
    public List<Answer> Answers { get; set; } = new();
}