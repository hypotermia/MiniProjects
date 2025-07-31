using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using MiniProjects.Interfaces;
using MiniProjects.Models;
using System.Text;

namespace MiniProjects.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly MasterServicesContext _context;

        public LoginRepository(MasterServicesContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string names)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Names == names);
        }

        public async Task<User> RegisterAsync(User user)
        {
            //var passwordHash = HashPassword(request.Password);
            var newuser = new User
            {
                Names = user.Names,
                Passwords = user.Passwords
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ValidateUserAsync(string name, string password)
        {
            var user = await GetByEmailAsync(name);
            if (user == null) return false;

            return true;
        }

        //private string HashPassword(string password)
        //{
        //    using var sha256 = SHA256.Create();
        //    var bytes = Encoding.UTF8.GetBytes(password);
        //    var hash = sha256.ComputeHash(bytes);
        //    return Convert.ToBase64String(hash);
        //}
    }
}
