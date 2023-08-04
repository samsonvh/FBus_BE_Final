namespace FBus_BE.DTOs.PageDTOs
{
    public class AccountPageRequest : DefaultPageRequest
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}
