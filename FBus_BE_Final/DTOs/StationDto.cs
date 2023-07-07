namespace FBus_BE.DTOs
{
    public class StationDto
    {
        public short Id { get; set; }
        public short? CreatedById { get; set; }
        public string CreatedByCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string? Image { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
