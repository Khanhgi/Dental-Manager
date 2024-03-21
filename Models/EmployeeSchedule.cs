using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class EmployeeSchedule
{
    public int EmployeeScheduleId { get; set; }

    public TimeSpan? Time { get; set; }

    public virtual ICollection<EmployeeScheduleDetail> EmployeeScheduleDetails { get; set; } = new List<EmployeeScheduleDetail>();
}
