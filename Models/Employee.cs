using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string EmployeeEmail { get; set; } = null!;

    public string EmployeePhone { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public int RoleId { get; set; }

    public int ClinicId { get; set; }

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
