using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    public decimal PaymentAmount { get; set; }

    public DateTime PaymentDate { get; set; }
}
