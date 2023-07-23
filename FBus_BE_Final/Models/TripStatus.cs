using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class TripStatus
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public short? TripId { get; set; }

    public short? StationId { get; set; }

    public byte? CountUp { get; set; }

    public byte? CountDown { get; set; }

    public byte StatusOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual Account? CreatedBy { get; set; }

    public virtual Station? Station { get; set; }

    public virtual Trip? Trip { get; set; }
}
