namespace FBus_BE.DTOs.ListingDTOs
{
    public class BusListingDto
    {
        public short Id { get; set; }
        public string Code { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public byte Seat { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
