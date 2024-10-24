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
    public interface ISalaryHistoryRepository: IBaseRepository<SalaryHistory>
    {
        Task<PagedDto<SalaryHistory>> GetPagedAsync(SalaryHistoryFilter filter);
    }
}