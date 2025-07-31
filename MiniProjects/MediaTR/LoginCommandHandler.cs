using MediatR;
using Microsoft.IdentityModel.Tokens;
using MiniProjects.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniProjects.MediaTR
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IConfiguration _config;
        private readonly ILoginRepository _userRepository;

        public LoginCommandHandler(ILoginRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var isValidUser = await _userRepository.ValidateUserAsync(request.names, request.Password);

            if (!isValidUser)
                throw new UnauthorizedAccessException("Invalid email or password.");

            // Ambil user dari email
            var user = await _userRepository.GetByEmailAsync(request.names);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Names),
            new Claim(ClaimTypes.Role, "User") // opsional
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
