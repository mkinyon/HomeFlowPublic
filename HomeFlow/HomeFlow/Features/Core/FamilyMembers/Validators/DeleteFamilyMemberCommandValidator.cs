namespace HomeFlow.Features.Core.FamilyMembers;

public class DeleteFamilyMemberCommandValidator : AbstractValidator<DeleteFamilyMemberCommand>
{
    public DeleteFamilyMemberCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Family member ID is required." );
    }
}