using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Route
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public string Beginning { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public short Distance { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual ICollection<Coordination> Coordinations { get; set; } = new List<Coordination>();

    public virtual Account? CreatedBy { get; set; }

    public virtual ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
}
