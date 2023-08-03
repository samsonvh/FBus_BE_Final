using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class BusInputDto
    {
        [Required]
        [MaxLength(9)]
        [MinLength(6)]
        [RegularExpression("^\\d{2}[A-Z]\\d{4,6}", ErrorMessage = "LicensePlate must follow format. For example: 01A012345")]
        public string LicensePlate { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Brand { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Model { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Color { get; set; }
        [Required]
        public byte Seat { get; set; }
        [Required]
        public DateTime? DateOfRegistration { get; set; }
    }
}
