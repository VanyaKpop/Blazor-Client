
namespace Blazor_Client.Models;

public class Answer
{
    public long Id { get; set; }
    public bool IsTrueAnswer { get; set; }
    public bool? UserAnswer { get; set;} = false;
    public string Body { get; set; } = "";
    public long QuestionId { get; set; }

    public string Fill {get; set;} = "";
}