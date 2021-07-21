using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.EntityFramework;

namespace TimeOffTracker.Model.Repositories
{
    public class UserRepository
    {
        public async Task<List<User>> SelectAlldAsync(UserDto filter, CancellationToken token)
        {
            await using var context = new MasterContext();
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
            await using var context = new MasterContext();
            return await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync(token);
        }

        public async Task<User> SelectByLoginAsync(string login, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.Users.Where(u => u.Login == login).FirstOrDefaultAsync(token);
        }

        public async Task<User> SelectByLoginAndPasswordAsync(string login, string password, CancellationToken token)
        {
            await using var context = new MasterContext();
            return await context.Users.Where(u => u.Login == login && u.Password == password)
                .FirstOrDefaultAsync(token);
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