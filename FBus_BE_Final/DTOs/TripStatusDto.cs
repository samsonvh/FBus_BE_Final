namespace FBus_BE.DTOs
{
    public class TripStatusDto
    {
        public short Id { get; set; }

        public short? CreatedById { get; set; }

        public short? TripId { get; set; }

        public short? StationId { get; set; }

        public StationDto? Station { get; set; }

        public byte? CountUp { get; set; }

        public byte? CountDown { get; set; }

        public byte StatusOrder { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; }
    }
}
