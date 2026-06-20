using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using FluentValidation;
using SplitRightApi.cs.Models;

namespace SplitRightApi.cs.Validators
{
    public class MemberValidator : AbstractValidator<AddMemberDto>
    { 
        public MemberValidator() {

            RuleFor(AddMemberDto => AddMemberDto.UserId)
                .GreaterThan(0)
                .WithMessage("Valid UserId Required");
        }
    }
}
