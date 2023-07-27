namespace FBus_BE.DTOs.PageDTOs
{
    public class StationPageRequest : DefaultPageRequest
    {
        public string? Code { get; set; }
        public string? Status { get; set; }
    }
}
