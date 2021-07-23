using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TimeOffTracker.Model;
using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Enum;
using TimeOffTracker.Model.Repositories;

namespace TimeOffTracker.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;

        public AuthController(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] AuthDto request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var user = await userRepository.SelectByLoginAndPasswordAsync(request.Login, request.Password, token);
            if (user == null)
            {
                return Unauthorized();
            }

            var actionResult = await GenerateJwt(user, token);
            var value = $"Bearer {actionResult.Value}";
            return Ok(value);
        }

        private async Task<ActionResult<string>> GenerateJwt(User user, CancellationToken cancellationToken)
        {
            var userRole = (UserRoles) user.RoleId;

            var authParams = _authOptions.Value;
            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, user.Login),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("role", userRole.ToString())
            };

            var token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}