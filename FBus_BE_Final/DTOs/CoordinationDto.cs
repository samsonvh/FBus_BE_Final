namespace FBus_BE.DTOs
{
    public class CoordinationDto
    {
        public short Id { get; set; }
        public short CreatedById { get; set; }
        public string CreatedByCode { get; set; }
        public short? DriverId { get; set; }
        public DriverDto? Driver { get; set; }
        public short? BusId { get; set; }
        public BusDto Bus { get; set; }
        public short? RouteId { get; set; }
        public RouteDto Route { get; set; }
        public string? Note { get; set; }
        public DateTime DateLine { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
