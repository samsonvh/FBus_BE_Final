namespace FBus_BE.DTOs.ListingDTOs
{
    public class DriverListingDto
    {
        public short Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
