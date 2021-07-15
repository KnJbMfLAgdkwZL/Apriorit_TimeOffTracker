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
using TimeOffTracker.Model.Repositories;

namespace TimeOffTracker.Controllers {
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly IOptions<AuthOptions> _authOptions;
        
        public AuthController(IOptions<AuthOptions> authOptions) {
            this._authOptions = authOptions;
        }
        
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<AuthDto>> Login([FromBody] AuthDto request, CancellationToken token) {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var user = await userRepository.SelectByLoginAsync(request.Login, token);

            if (user == null) return Unauthorized();
            var jwtToken = GenerateJwt(user);

            return Ok(new {
                    access_token = jwtToken
                }
            );

        }

        private string GenerateJwt(User user) {
            var authParams = _authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new (JwtRegisteredClaimNames.UniqueName, user.Login),
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new ("role", user.RoleId.ToString())
            };


            var token = new JwtSecurityToken(authParams.Issuer, authParams.Audience, claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime), signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}