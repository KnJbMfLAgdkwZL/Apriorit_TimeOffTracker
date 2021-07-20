using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeOffTracker.Model.Repositories
{
    public class UserRoleRepository
    {
        public async Task<UserRole> SelectByIdAsync(int id, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.UserRoles.Where(ur => ur.Id == id && ur.Deleted == false).FirstOrDefaultAsync(token);
        }
    }
}