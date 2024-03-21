using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class EmployeeScheduleDetail
{
    public int EmployeeScheduleId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? Date { get; set; }

    public bool? Status { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual EmployeeSchedule EmployeeSchedule { get; set; } = null!;
}
