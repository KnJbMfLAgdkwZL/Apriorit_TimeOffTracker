using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Repositories;

namespace TimeOffTracker.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        [HttpGet("get/{id:long}")]
        public async Task<ActionResult<UserDto>> Get([FromRoute(Name = "id")] int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var user = await userRepository.SelectByIdAsync(id, token);
            if (user == null)
            {
                return NoContent();
            }

            var userDto = Converter.EntityToDto(user);
            return Ok(userDto);
        }

        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost("create")]
        public async Task<ActionResult<int>> Create([FromBody] UserDto user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRoleRepository = new UserRoleRepository();
            var userRole = await userRoleRepository.SelectByIdAsync(user.RoleId, token);
            if (userRole == null)
            {
                return BadRequest("Wrong Role");
            }

            var userRepository = new UserRepository();
            var r = await userRepository.InsertAsync(user, token);
            return Ok(r);
        }
    }
}