namespace Blazor_Client.Models;

public record LoginRequest(string token);

public class UserRequest
{
    public string name { get; set; } = "";
    public string password { get; set; } = "";
}


public class TestRequest
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public required string DataJson { get; set; }
}