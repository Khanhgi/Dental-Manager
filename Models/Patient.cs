using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string PatientPhone { get; set; } = null!;

    public string PatientEmail { get; set; } = null!;

    public string PatientAddress { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual Role Role { get; set; } = null!;
}
