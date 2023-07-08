using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Bus
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public string Code { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Color { get; set; } = null!;

    public byte Seat { get; set; }

    public DateTime? DateOfRegistration { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual ICollection<Coordination> Coordinations { get; set; } = new List<Coordination>();

    public virtual Account? CreatedBy { get; set; }
}
