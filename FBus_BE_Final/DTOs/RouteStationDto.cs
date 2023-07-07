namespace FBus_BE.DTOs
{
    public class RouteStationDto
    {
        public short Id { get; set; }
        public short? RouteId { get; set; }
        public RouteDto? Route { get; set; }
        public short? StationId { get; set; }
        public StationDto? Station { get; set; }
        public byte StationOrder { get; set; }

    }
}
