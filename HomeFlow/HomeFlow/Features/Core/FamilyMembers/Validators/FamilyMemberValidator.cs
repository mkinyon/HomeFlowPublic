namespace HomeFlow.Features.Core.FamilyMembers
{
    public class FamilyMemberValidator : AbstractValidator<FamilyMember>
    {
        public FamilyMemberValidator()
        {
            RuleFor( x => x.FirstName )
                .NotEmpty().WithMessage( "The first name is required." )
                .MaximumLength( 50 ).WithMessage( "The first name cannot exceed 50 characters." );

            RuleFor( x => x.LastName )
                .NotEmpty().WithMessage( "The last name is required." )
                .MaximumLength( 50 ).WithMessage( "The last name cannot exceed 50 characters." );
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
        {
            var result = await ValidateAsync( ValidationContext<FamilyMember>.CreateWithOptions( (FamilyMember) model, x => x.IncludeProperties( propertyName ) ) );
            if ( result.IsValid )
            {
                return Array.Empty<string>();
            }

            return result.Errors.Select( e => e.ErrorMessage );
        };
    }
}
