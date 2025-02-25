
namespace Blazor_Client.Models;

public class Answer
{
    public long Id { get; set; }
    public bool IsTrueAnswer { get; set; }
    public bool? UserAnswer { get; set;} 
    public string Body { get; set; } = "";
    public long QuestionId { get; set; }
}