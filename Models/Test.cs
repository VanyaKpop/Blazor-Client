namespace Blazor_Client.Models;

public class Test
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Author { get; set; }
    public List<Question> Questions { get; set; }
}
