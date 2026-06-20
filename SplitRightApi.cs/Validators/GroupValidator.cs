using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using FluentValidation;
using SplitRightApi.cs.Models;
namespace SplitRightApi.cs.Validators
{
    public class GroupValidator : AbstractValidator<CreateGroupDto>
    {
        public GroupValidator() {

            RuleFor(CreateGroupDto => CreateGroupDto.Name)
              .NotEmpty()
              .WithMessage("Group Name id Required")
              .MaximumLength(50)
              .WithMessage("Name Should Not Be More Than 50 Characters");


            RuleFor(CreateGroupDto => CreateGroupDto.Description)
                .NotEmpty()
                .WithMessage("Description Should Not Be Empty")
                .MaximumLength(100);

        }
    }
}
