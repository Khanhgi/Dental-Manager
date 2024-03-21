using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int EmployeeId { get; set; }

    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeSpan AppointmentTime { get; set; }

    public int ClinicId { get; set; }

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
