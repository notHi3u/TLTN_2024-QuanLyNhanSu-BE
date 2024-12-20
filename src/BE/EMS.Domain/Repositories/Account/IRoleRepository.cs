﻿using Common.Data;
using Common.Dtos;
using EMS.Domain.Filters.Account;
using EMS.Domain.Models.Account;

namespace EMS.Domain.Repositories.Account
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetByIdAsync(string id, bool? isDeep);
        Task<PagedDto<Role>> GetPagedAsync(RoleFilter filter);
        Task<IEnumerable<string>> GetIdByNameAsync(IEnumerable<string> name);
    }
}
