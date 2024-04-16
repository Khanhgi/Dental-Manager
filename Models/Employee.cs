using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EmployeePassword { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeePhone { get; set; }

    public string? EmployeeAddress { get; set; }

    public string? Avatar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public int? RoleId { get; set; }

    public int? ClinicId { get; set; }

    public int? FailedLoginAttempt { get; set; }

    public DateTime? LastFailedLoginAttempt { get; set; }

    public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; } = new List<AppointmentDetail>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Clinic? Clinic { get; set; }

    public virtual ICollection<EmployeeScheduleDetail> EmployeeScheduleDetails { get; set; } = new List<EmployeeScheduleDetail>();

    public virtual Role? Role { get; set; }
}
