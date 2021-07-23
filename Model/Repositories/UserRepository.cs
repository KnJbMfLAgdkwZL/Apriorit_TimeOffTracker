using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TimeOffTracker.Model.Repositories
{
    public class UserRepository
    {
        public async Task<List<User>> SelectAllAsync(UserDto filter, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u =>
                EF.Functions.Like(u.Email, $"%{filter.Email}%") &&
                EF.Functions.Like(u.Login, $"%{filter.Login}%") &&
                EF.Functions.Like(u.FirstName, $"%{filter.FirstName}%") &&
                EF.Functions.Like(u.SecondName, $"%{filter.SecondName}%") &&
                u.Deleted == false
            ).ToListAsync(token);
        }

        public async Task<User> SelectByIdAsync(int id, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u => u.Id == id && u.Deleted == false).FirstOrDefaultAsync(token);
        }

        public async Task<User> SelectByLoginAsync(string login, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u => u.Login == login && u.Deleted == false).FirstOrDefaultAsync(token);
        }

        public async Task<User> SelectByEmailAsync(string email, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u => u.Email == email && u.Deleted == false).FirstOrDefaultAsync(token);
        }

        public async Task<User> SelectByLoginAndPasswordAsync(string login, string password, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u => u.Login == login && u.Password == password && u.Deleted == false)
                .FirstOrDefaultAsync(token);
        }

        public async Task<int> InsertAsync(UserDto userDto, CancellationToken token)
        {
            await using var context = new masterContext();
            var user = Converter.DtoToEntity(userDto);
            await context.Users.AddAsync(user, token);
            await context.SaveChangesAsync(token);
            return user.Id;
        }

        public async Task<bool> UpdateRoleIdAsync(int userId, int roleId, CancellationToken token)
        {
            await using var context = new masterContext();
            var user = await SelectByIdAsync(userId, token);
            if (user == null)
            {
                return false;
            }

            user.RoleId = roleId;
            context.Users.Update(user);
            await context.SaveChangesAsync(token);
            return true;
        }

        public async Task<User> SelectAccounting(CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Users.Where(u => u.RoleId == 2 && u.Deleted == false).FirstOrDefaultAsync(token);
        }
    }
}