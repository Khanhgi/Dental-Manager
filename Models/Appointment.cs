using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? EmployeeId { get; set; }

    public int? PatientId { get; set; }

    public int? ClinicId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Note { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public bool? Status { get; set; }

    public DateTime? AppointmentCreatedDate { get; set; }

    public bool? IsBooking { get; set; }

    public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; } = new List<AppointmentDetail>();

    public virtual Clinic? Clinic { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
