namespace FBus_BE.DTOs
{
    public class ErrorDto
    {
        public string Title { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
