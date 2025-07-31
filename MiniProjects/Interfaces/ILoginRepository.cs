using Microsoft.AspNetCore.Identity.Data;
using MiniProjects.Models;
namespace MiniProjects.Interfaces
{
    public interface ILoginRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> RegisterAsync(User user);
        Task<bool> ValidateUserAsync(string email, string password);
    }
}
