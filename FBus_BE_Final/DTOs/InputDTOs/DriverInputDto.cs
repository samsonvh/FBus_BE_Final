using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class DriverInputDto
    {
        [MaxLength(100)]
        [MinLength(10)]
        [RegularExpression("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+[.][a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9]+)*$", ErrorMessage = "Email must be in valid format")]
        public string? Email { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Code { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string FullName { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        [MaxLength(12)]
        [MinLength(6)]
        public string IdCardNumber { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(20)]
        public string Address { get; set; }
        [Required]
        [MaxLength(13)]
        [RegularExpression("^[+]\\d{2} \\d{6,9}", ErrorMessage = "PhoneNumber must follow format. For example: +84 987654321")]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        [MinLength(10)]
        [RegularExpression("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$", ErrorMessage = "Email must be in valid format")]
        public string? PersonalEmail { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
