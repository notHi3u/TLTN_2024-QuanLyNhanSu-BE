﻿using Common.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Filters.EMS
{
    public class LeaveRequestFilter: FilterBase
    {
        public string? EmployeeId { get; set; }
        public string? LeaveType { get; set; }
    }
}
