using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class MedicalHistory
{
    public int HistoryId { get; set; }

    public int PatientId { get; set; }

    public string HistoryDetails { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
