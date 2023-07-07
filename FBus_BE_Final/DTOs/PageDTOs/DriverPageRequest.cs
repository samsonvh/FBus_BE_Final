namespace FBus_BE.DTOs.PageDTOs
{
    public class DriverPageRequest : DefaultPageRequest
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
    }
}
