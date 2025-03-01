namespace Blazor_Client.Models;

public class Question
{
    public required bool IsRequired { get; set; }
    public required string Body { get; set; }
    public List<Answer> Answers { get; set; } = new();

    public Question() {
        Answers.Add(new Answer());
    }
}