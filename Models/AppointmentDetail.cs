using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class AppointmentDetail
{
    public int AppointmentId { get; set; }

    public int ServiceId { get; set; }

    public double? Price { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
