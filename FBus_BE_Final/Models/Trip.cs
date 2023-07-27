using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Trip
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public short? DriverId { get; set; }

    public short? BusId { get; set; }

    public short? RouteId { get; set; }

    public string? Note { get; set; }

    public DateTime DateLine { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public DateTime DueDate { get; set; }

    public virtual Bus? Bus { get; set; }

    public virtual Account? CreatedBy { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual Route? Route { get; set; }

    public virtual ICollection<TripStatus> TripStatuses { get; set; } = new List<TripStatus>();
}
