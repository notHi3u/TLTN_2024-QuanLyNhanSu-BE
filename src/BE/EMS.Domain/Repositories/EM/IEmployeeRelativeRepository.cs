﻿using Common.Data;
using Common.Dtos;
using EMS.Domain.Filters.EMS;
using EMS.Domain.Models.EM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repositories.EM
{
    public interface IEmployeeRelativeRepository: IBaseRepository<EmployeeRelative>
    {
        Task<PagedDto<EmployeeRelative>> GetPagedAsync(EmployeeRelativeFilter filter);
        Task<IEnumerable<EmployeeRelative>> GetByEmployeeIdAsync(string employeeId);
    }
}
