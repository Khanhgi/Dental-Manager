using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string PatientPassword { get; set; } = null!;

    public string? PatientName { get; set; }

    public string? PatientPhone { get; set; }

    public string? PatientEmail { get; set; }

    public string? PatientAddress { get; set; }

    public int? RoleId { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual Role? Role { get; set; }
}
