using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeOffTracker.Model.Repositories
{
    public class RequestTypeRepository
    {
        public async Task<List<RequestType>> SelectAllAsync(CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.RequestTypes.Where(rt => rt.Deleted == false).ToListAsync(token);
        }

        public async Task<RequestType> SelectOneByIdAsync(int id, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.RequestTypes.Where(rt => rt.Id == id && rt.Deleted == false)
                .FirstOrDefaultAsync(token);
        }
    }
}