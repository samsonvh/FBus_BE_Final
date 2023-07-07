namespace FBus_BE.DTOs
{
    public class DriverDto
    {
        public short Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public short? CreatedById { get; set; }
        public string? CreatedByCode { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; }
        public string IdCardNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? PersonalEmail { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
