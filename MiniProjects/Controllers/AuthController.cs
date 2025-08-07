using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniProjects.MediaTR;
using MiniProjects.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MiniProjects.Repository.WebApiHelper;

namespace MiniProjects.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;

        public AuthController(IMediator mediator, IConfiguration config )
        {
            _mediator = mediator;
            _config = config;
        }
        private User? AuthenticateUser(LoginCommand login)
        {
            if (login.names == "admin" && login.Password == "admin123")
            {
                return new User { Id = 1, Names = "admin" };
            }

            return null;
        }
        [HttpPost]
        public async Task<ApiResponseObj> Login([FromBody] LoginCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);
                return new ApiResponseObj
                {
                    Success = true,
                    message = "Success Login!!",
                    transactionId = "",
                    data = token,
                    status = true
                }; 
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ApiResponseObj
                {
                    Success = true,
                    message = ex.Message,
                    transactionId = "",
                    data = null,
                    status = false
                };
            }
        }
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Names!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
