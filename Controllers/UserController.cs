using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("get/{id:int}")]
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


        ///////////////////////////////////////
        ////
        ////
        //// 
        [Route("isLogined")]
        [HttpGet]
        [Authorize]
        public ActionResult<string> isLogined()
        {
            /*foreach (var v in User.Claims)
            {
                Console.WriteLine(v.Type);
                Console.WriteLine(v.Value);
                Console.WriteLine();
            }*/

            var user_id = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Console.WriteLine("User.Identity.Name: " + User.Identity.Name);
            Console.WriteLine("User.Identity.AuthenticationType: " + User.Identity.AuthenticationType);
            Console.WriteLine("User.Identity.IsAuthenticated: " + User.Identity.IsAuthenticated);

            return Ok("isLogined true id = " + user_id);
        }

        [Route("isLoginedAdmin")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> isLoginedAdmin()
        {
            return Ok("isLoginedAdmin true");
        }

        [Route("isLoginedUser")]
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult<string> isLoginedUser()
        {
            return Ok("isLoginedUser true");
        }

        [Route("home")]
        [HttpGet]
        public ActionResult<string> home()
        {
            if (User != null)
            {
                if (User.Identity != null)
                {
                    Console.WriteLine("User.Identity.Name: " + User.Identity.Name);
                    Console.WriteLine("User.Identity.AuthenticationType: " + User.Identity.AuthenticationType);
                    Console.WriteLine("User.Identity.IsAuthenticated: " + User.Identity.IsAuthenticated);
                }

                if (User.Claims != null)
                {
                    var user_id = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    Console.WriteLine("isLogined true id = " + user_id);
                }
            }

            return Ok("home user_id = ");
        }
        ////
        ////
        ////
        ////////////////////////////////////////////
    }
}