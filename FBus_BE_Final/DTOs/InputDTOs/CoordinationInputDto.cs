using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class CoordinationInputDto
    {
        [Required]
        public short DriverId { get; set; }
        [Required]
        public short BusId { get; set; }
        [Required]
        public short RouteId { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }
        [Required]
        public DateTime DateLine { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}
