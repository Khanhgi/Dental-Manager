using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int RoleId { get; set; }

    public string DoctorName { get; set; } = null!;

    public string DoctorSpecialty { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Role Role { get; set; } = null!;
}
