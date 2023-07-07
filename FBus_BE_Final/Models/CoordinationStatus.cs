using System;
using System.Collections.Generic;

namespace FBus_BE.Models;

public partial class CoordinationStatus
{
    public short Id { get; set; }

    public short? CreatedById { get; set; }

    public short? CoordinationId { get; set; }

    public byte OriginalStatus { get; set; }

    public byte UpdatedStatus { get; set; }

    public byte StatusOrder { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Coordination? Coordination { get; set; }

    public virtual Account? CreatedBy { get; set; }
}
