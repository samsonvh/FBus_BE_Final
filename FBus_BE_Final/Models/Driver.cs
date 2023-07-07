using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class Driver
{
    public short Id { get; set; }

    public short? AccountId { get; set; }

    public short? CreatedById { get; set; }

    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public string IdCardNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? PersonalEmail { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Avatar { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Coordination> CoordinationBus { get; set; } = new List<Coordination>();

    public virtual ICollection<Coordination> CoordinationDrivers { get; set; } = new List<Coordination>();

    public virtual ICollection<Coordination> CoordinationRoutes { get; set; } = new List<Coordination>();

    public virtual Account? CreatedBy { get; set; }
}
