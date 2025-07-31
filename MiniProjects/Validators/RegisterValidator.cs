using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using MiniProjects.Models;

namespace MiniProjects.Validators
{


    public class RegisterValidator : AbstractValidator<User>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Names).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Names).NotEmpty().EmailAddress();
            RuleFor(x => x.Passwords).NotEmpty().MinimumLength(6);
        }
    }
}
