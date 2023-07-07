namespace FBus_BE.DTOs
{
    public class RouteDto
    {
        public short Id { get; set; }
        public short CreatedById { get; set; }
        public string CreatedByCode { get; set; }
        public string Beginning { get; set; }
        public string Destination { get; set; }
        public short Distance { get; set; }
        public List<RouteStationDto> Stations { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
