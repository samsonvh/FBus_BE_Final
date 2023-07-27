namespace FBus_BE.DTOs.PageDTOs
{
    public class RoutePageRequest : DefaultPageRequest
    {
        public string? Beginning { get; set; }
        public string? Destination { get; set; }
        public string? Status { get; set; }
    }
}
