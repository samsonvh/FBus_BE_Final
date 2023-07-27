using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class TripInputDto
    {
        [Required]
        public short DriverId { get; set; }
        [Required]
        public short BusId { get; set; }
        [Required]
        public short RouteId { get; set; }
        public string? Note { get; set; }
        [Required]
        public DateTime DateLine { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}
