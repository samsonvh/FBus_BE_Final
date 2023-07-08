using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class StationInputDto
    {
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string Code { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string AddressNumber { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Street { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Ward { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        public string District { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(4)]
        public string City { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public float Latitude { get; set; }
    }
}
