using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Station
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string AddressNumber { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Image { get; set; }

    public float Longitude { get; set; }

    public float Latitude { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual ICollection<BusTripStatus> BusTripStatuses { get; set; } = new List<BusTripStatus>();

    public virtual Account? CreatedBy { get; set; }

    public virtual ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
}
