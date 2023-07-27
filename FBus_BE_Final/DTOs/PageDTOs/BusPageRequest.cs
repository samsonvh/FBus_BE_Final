namespace FBus_BE.DTOs.PageDTOs
{
    public class BusPageRequest : DefaultPageRequest
    {
        public string? Code { get; set; }
        public string? LicensePlate { get; set; }
        public string? Status { get; set; }
    }
}
