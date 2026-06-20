using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using FluentValidation;
using SplitRightApi.cs.Models;

namespace SplitRightApi.cs.Validators
{
    public class RegisterValidator :AbstractValidator<RegisterDto>
    {
        public RegisterValidator() {

            RuleFor(RegisterDto => RegisterDto.Name)
                .NotEmpty()
                .WithMessage("Name Should Not Be Empty");

            RuleFor(RegisterDto => RegisterDto.Email)
                .NotEmpty()
                .WithMessage("Email Should Not Be Empty")
                .EmailAddress()
                .WithMessage("Enter a Valid Mail Id");

            RuleFor(RegisterDto => RegisterDto.Password)
                .NotEmpty()
                .WithMessage("Password Should not be Empty")
                .MinimumLength(6)
                .WithMessage("The Password Should Be Minimum Six Characters");

                 

        
        
        }
    }
}
