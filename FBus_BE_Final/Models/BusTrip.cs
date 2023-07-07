using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class BusTrip
{
    public short Id { get; set; }

    public short? CoordinationId { get; set; }

    public DateTime StartingDate { get; set; }

    public DateTime? EndingDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual ICollection<BusTripStatus> BusTripStatuses { get; set; } = new List<BusTripStatus>();

    public virtual Coordination? Coordination { get; set; }
}
