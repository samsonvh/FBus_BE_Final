using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Account
{
    public short Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string Code { get; set; } = null!;

    public string Role { get; set; } = null!;

    public byte Status { get; set; }

    public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();

    public virtual ICollection<Driver> DriverAccounts { get; set; } = new List<Driver>();

    public virtual ICollection<Driver> DriverCreatedBies { get; set; } = new List<Driver>();

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();

    public virtual ICollection<TripStatus> TripStatuses { get; set; } = new List<TripStatus>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
