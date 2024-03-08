using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int AppointmentId { get; set; }

    public string PrescriptionDetails { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;
}
