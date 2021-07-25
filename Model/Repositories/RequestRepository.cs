using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Model.Repositories
{
    public class RequestRepository
    {
        public async Task<int> InsertAsync(RequestDto requestDto, CancellationToken token)
        {
            await using var context = new masterContext();
            var request = Converter.DtoToEntity(requestDto);
            await context.Requests.AddAsync(request, token);
            await context.SaveChangesAsync(token);
            return request.Id;
        }

        public async Task<Request> SelectByIdAndUserIdAsync(int id, int userId, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests
                .Where(r => r.Id == id && r.UserId == userId && r.StateDetailId != (int) StateDetails.Deleted)
                .Include(r => r.UserSignatures
                    .Where(us => us.Deleted == false))
                .FirstOrDefaultAsync(token);
        }
    }
}