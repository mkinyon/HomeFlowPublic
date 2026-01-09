namespace HomeFlow.Features.Core.FamilyMembers;

public class CreateFamilyMemberCommandValidator : AbstractValidator<CreateFamilyMemberCommand>
{
    public CreateFamilyMemberCommandValidator()
    {
        RuleFor( x => x.FamilyMember )
            .NotNull().WithMessage( "Family member is required." );

        When( x => x.FamilyMember != null, () =>
        {
            RuleFor( x => x.FamilyMember.FirstName )
                .NotEmpty().WithMessage( "The first name is required." )
                .MaximumLength( 50 ).WithMessage( "The first name cannot exceed 50 characters." );

            RuleFor( x => x.FamilyMember.LastName )
                .NotEmpty().WithMessage( "The last name is required." )
                .MaximumLength( 50 ).WithMessage( "The last name cannot exceed 50 characters." );
        } );
    }
}