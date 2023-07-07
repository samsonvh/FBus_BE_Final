namespace FBus_BE.DTOs
{
    public class AccountDto
    {
        public short Id { get; set; }

        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
