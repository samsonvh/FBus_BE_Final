namespace FBus_BE.DTOs.PageDTOs
{
    public class DefaultPageResponse<T>
    {
        public List<T>? Data { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
    }
}
