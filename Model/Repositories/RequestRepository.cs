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
        public async Task<List<Request>> SelectAllAsync(RequestDto filter, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests.Where(r =>
                r.UserId == filter.UserId &&
                (filter.RequestTypeId == 0 || r.RequestTypeId == (int) filter.RequestTypeId) &&
                (filter.StateDetailId == 0 || r.StateDetailId == (int) filter.StateDetailId) &&
                EF.Functions.Like(r.Reason, $"%{filter.Reason}%")
            ).ToListAsync(token);
        }

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

        public async Task UpdateAsync(Request request, CancellationToken token)
        {
            await using var context = new masterContext();
            context.Requests.Update(request);
            await context.SaveChangesAsync(token);
        }

        public async Task<Request> SelectByIdAndUserIdAsync(int id, int userId, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests
                .Where(r =>
                    r.Id == id &&
                    r.UserId == userId &&
                    r.StateDetailId != (int) StateDetails.Deleted
                )
                .Include(r => r.UserSignatures
                    .Where(us => us.Deleted == false))
                .FirstOrDefaultAsync(token);
        }

        public async Task<Request> SelectAsync(int id, int userId, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests
                .Where(r =>
                    r.Id == id &&
                    r.UserId == userId
                )
                .Include(r => r.UserSignatures
                    .Where(us => us.Deleted == false))
                .FirstOrDefaultAsync(token);
        }

        public async Task<Request> SelectAsync(int id, CancellationToken token)
        {
            await using var context = new masterContext();
            return await context.Requests.Where(r => r.Id == id)
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

        public async Task<TimeSpan> GetDays(int userId, int requestTypesId, CancellationToken token)
        {
            var beginYear = new DateTime(DateTime.Now.Year, 1, 1);

            await using var context = new masterContext();
            var requests = await context.Requests.Where(r =>
                r.UserId == userId &&
                r.RequestTypeId == requestTypesId &&
                r.StateDetailId == (int) StateDetails.Approved &&
                r.DateTimeFrom >= beginYear
            ).Select(r => r.DateTimeTo - r.DateTimeFrom).ToListAsync(token);

            var sum = new TimeSpan(0, 0, 0, 0, 0);
            foreach (var r in requests)
            {
                sum += r;
            }

            return sum;
        }
    }
}