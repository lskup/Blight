using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Blight.Infrastructure;
using Microsoft.AspNetCore.Identity;


namespace Blight.Models.Validators
{
    public class RegisterUserDtoValidator:AbstractValidator<RegisterUserDto>
    {

        public RegisterUserDtoValidator(BlightDbContext blightDbContext)
        {
            RuleFor(s => s.FirstName)
                .NotEmpty()
                .Length(2, 30);

            RuleFor(s => s.LastName)
                .NotEmpty()
                .Length(2, 30);

            RuleFor(s => s.Email)
                .NotEmpty()
                .EmailAddress()
                .Custom((value, context) =>
                {
                    var emailExisting = blightDbContext.Users.Any(x => x.Email == value);
                    if(emailExisting)
                    {
                        context.AddFailure($"Email {value} is occupied");
                    }
                });

            RuleFor(s => s.Password)
                .Custom((password, context) =>
                {
                    if (password.Length < 6)
                    {
                        context.AddFailure("Password requires minimum 6 marks");
                    }
                    if (!password.Any(char.IsUpper))
                    {
                        context.AddFailure("Password requires at least 1 Uppercase letter");
                    }
                    if(!password.Any(char.IsDigit))
                    {
                        context.AddFailure("Password requires at least 1 digit");
                    }

                });

            RuleFor(s => s.PasswordConfirmation)
                .NotEmpty()
                .Matches(c => c.Password)
                .WithMessage("Password Confirmation pole must match password");

            RuleFor(s => s.DateOfBirth)
                .NotEmpty();
                
        }
    }
}
