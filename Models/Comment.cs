namespace Blazor_Client.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public required string Author { get; set; }
        public long TestId { get; set; }
        public required string TestComment { get; set; }
        public TestRequest? Test { get; set; }
    }
}
