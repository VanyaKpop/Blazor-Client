namespace Blazor_Client.Models;

public class User
{
    public long id { get; set; }
    public required string name { get; set; }
    public required string password { get; set; }
    public required string role { get; set; }
}