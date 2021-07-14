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
            var userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.Id == id, cancellationToken: token);
            return userRole;
        }
    }
}