using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int RoleId { get; set; }

    public int ClinicId { get; set; }

    public bool IsDeleted { get; set; }

    public string? DoctorName { get; set; }

    public string? DoctorSpecialty { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
