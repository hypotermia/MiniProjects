using MediatR;
namespace MiniProjects.MediaTR
{
    public class LoginCommand : IRequest<string>
    {
        public string names { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
