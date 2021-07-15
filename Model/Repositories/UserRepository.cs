using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TimeOffTracker.Model.Repositories
{
    public class UserRepository
    {
        public async Task<User> SelectByIdAsync(int id, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync(token);
        }
        
        public async Task<User> SelectByLoginAsync(string login, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.Users.Where(u => u.Login == login).FirstOrDefaultAsync(token);
        }

        public async Task<int> InsertAsync(UserDto userDto, CancellationToken token)
        {
            await using var context = new MasterContext();
            var user = Converter.DtoToEntity(userDto);
            await context.Users.AddAsync(user, token);
            await context.SaveChangesAsync(token);
            return user.Id;
        }
    }
}