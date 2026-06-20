using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRight.API.Data;
using FluentValidation;
using SplitRightApi.cs.Models;
using SplitRightApi.cs.Models.Entities;

namespace SplitRightApi.cs.Validators
{
    public class ExpenseValidator : AbstractValidator<CreateExpenseDto>
    {
        public ExpenseValidator()
        {
            RuleFor( CreateExpenseDto => CreateExpenseDto.Description)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Mandatory field");

            RuleFor(CreateExpenseDto => CreateExpenseDto.Category)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Mandatory Field");

            RuleFor(CreateExpenseDto => CreateExpenseDto.Amount)
                .NotEmpty()
                .WithMessage("Mandatory Field")
                .GreaterThan(0)
                .WithMessage("The Amount Should Be More Than 0");
        }
    }
}
