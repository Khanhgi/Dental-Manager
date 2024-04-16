using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public int ServiceTypeId { get; set; }

    public string? ServiceName { get; set; }

    public double? ServicePrice { get; set; }

    public bool? ServiceStatus { get; set; }

    public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; } = new List<AppointmentDetail>();

    public virtual ServiceType ServiceType { get; set; } = null!;
}
