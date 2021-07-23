using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeOffTracker.Model.Repositories
{
    public class ProjectRoleTypeRepository
    {
        public async Task<List<ProjectRoleType>> SelectAllAsync(CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.ProjectRoleTypes.Where(rt => rt.Deleted == false).ToListAsync(token);
        }

        public async Task<ProjectRoleType> SelectOneByIdAsync(int id, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.ProjectRoleTypes.Where(rt => rt.Id == id && rt.Deleted == false)
                .FirstOrDefaultAsync(token);
        }
    }
}