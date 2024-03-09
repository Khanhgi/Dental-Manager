using System;
using System.Collections.Generic;

namespace Dental_Manager.Models;

public partial class ServiceType
{
    public int ServiceTypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
