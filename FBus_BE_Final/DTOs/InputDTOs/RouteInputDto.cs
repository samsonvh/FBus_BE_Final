using System.ComponentModel.DataAnnotations;

namespace FBus_BE.DTOs.InputDTOs
{
    public class RouteInputDto
    {
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Beginning { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Destination { get; set; }
        [Required]
        public short Distance { get; set; }
        public List<int>? StationIds { get; set; }
    }
}
