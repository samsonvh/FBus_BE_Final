namespace FBus_BE.DTOs.ListingDTOs
{
    public class RouteListingDto
    {
        public short Id { get; set; }
        public string Beginning { get; set; }
        public string Destination { get; set; }
        public short Distance { get; set; }
        public string Status { get; set; }
    }
}
