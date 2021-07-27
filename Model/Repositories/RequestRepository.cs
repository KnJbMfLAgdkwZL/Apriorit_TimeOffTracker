using System;
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

        public async Task<int> UpdateAsync(RequestDto requestDto, CancellationToken token)
        {
            await using var context = new masterContext();
            var request = Converter.DtoToEntity(requestDto);
            context.Requests.Update(request);
            await context.SaveChangesAsync(token);
            return request.Id;
        }

        public async Task<Request> SelectByIdAndUserIdAsync(int id, int userId, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests
                .Where(r =>
                    r.Id == id &&
                    r.UserId == userId &&
                    r.StateDetailId != (int) StateDetails.Deleted &&
                    r.StateDetailId != (int) StateDetails.DeclinedByOwner
                )
                .Include(r => r.UserSignatures
                    .Where(us => us.Deleted == false))
                .FirstOrDefaultAsync(token);
        }

        public async Task<Request> CheckDateCollision(RequestDto request, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests
                .Where(r =>
                    r.UserId == request.UserId &&
                    r.StateDetailId != (int) StateDetails.Deleted &&
                    r.StateDetailId != (int) StateDetails.DeclinedByOwner &&
                    r.DateTimeTo >= DateTime.Now &&
                    (
                        r.DateTimeFrom <= request.DateTimeFrom &&
                        request.DateTimeFrom <= r.DateTimeTo
                        ||
                        r.DateTimeFrom <= request.DateTimeTo &&
                        request.DateTimeTo <= r.DateTimeTo
                    )
                ).FirstOrDefaultAsync(token);
        }

        public async Task DeleteOwnerAsync(int id, CancellationToken token)
        {
            await using var context = new masterContext();
            var requests = await context.Requests.Where(r =>
                r.Id == id &&
                r.StateDetailId != (int) StateDetails.Deleted &&
                r.StateDetailId != (int) StateDetails.DeclinedByOwner
            ).ToListAsync(token);

            foreach (var r in requests)
            {
                r.StateDetailId = (int) StateDetails.DeclinedByOwner;
                context.Requests.Update(r);
            }

            await context.SaveChangesAsync(token);
        }
    }
}