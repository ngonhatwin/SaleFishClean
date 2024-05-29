using FluentValidation;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Infrastructure.Validation
{
    public class UserRequestValidation : AbstractValidator<UserRequest>
    {
        public UserRequestValidation()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage("Username must not be empty.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Email must not be empty.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("Password must not be empty.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(p => p.RePassword)
                .NotEmpty().WithMessage("Re-enter password must not be empty.")
                .Equal(p => p.Password).WithMessage("Passwords must match.");
        }
    }

}
