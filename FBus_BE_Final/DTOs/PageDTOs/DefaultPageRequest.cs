namespace FBus_BE.DTOs.PageDTOs
{
    public class DefaultPageRequest
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
        public string? Direction { get; set; }
    }
}
