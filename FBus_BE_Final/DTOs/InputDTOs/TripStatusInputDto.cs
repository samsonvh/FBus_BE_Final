using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class TripStatusInputDto
    {
        [Required]
        public short? TripId { get; set; }
        [Required]
        public short? StationId { get; set; }
        public byte? CountUp { get; set; }
        public byte? CountDown { get; set; }
        public bool? IsFinished { get; set; }
    }
}
