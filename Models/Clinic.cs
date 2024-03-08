using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Clinic
{
    public int ClinicId { get; set; }

    public string ClinicName { get; set; } = null!;

    public string ClinicAddress { get; set; } = null!;

    public string ClinicPhone { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
