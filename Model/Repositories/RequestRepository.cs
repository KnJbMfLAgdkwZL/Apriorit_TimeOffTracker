using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
    }
}