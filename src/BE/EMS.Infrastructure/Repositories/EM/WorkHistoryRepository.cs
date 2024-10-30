﻿using Common.Data;
using Common.Dtos;
using EMS.Domain.Filters.EMS;
using EMS.Domain.Models.EM;
using EMS.Domain.Repositories.EM;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Infrastructure.Repositories.EM
{
    public class WorkHistoryRepository : BaseRepository<WorkHistory>, IWorkHistoryRepository
    {
        public WorkHistoryRepository(AppDbContext context, ILogger<WorkHistoryRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<PagedDto<WorkHistory>> GetPagedAsync(WorkHistoryFilter filter)
        {
            filter.PageIndex ??= 1;
            filter.PageSize ??= 10;

            var query = _dbSet.AsQueryable()
                .Include(wh => wh.Employee); // Include related Employee data if needed

            // Apply filtering based on filter properties
            if (!string.IsNullOrWhiteSpace(filter.EmployeeId))
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkHistory, Employee>)query.Where(wh => wh.EmployeeId == filter.EmployeeId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Position))
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkHistory, Employee>)query.Where(wh => wh.Position.Contains(filter.Position));
            }

            // Filtering for start and end dates if applicable
            if (filter.StartDate.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkHistory, Employee>)query.Where(wh => wh.StartDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkHistory, Employee>)query.Where(wh => wh.EndDate <= filter.EndDate.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value)
                .ToListAsync();

            return new PagedDto<WorkHistory>(items, totalCount, filter.PageIndex.Value, filter.PageSize.Value);
        }
    }
}
