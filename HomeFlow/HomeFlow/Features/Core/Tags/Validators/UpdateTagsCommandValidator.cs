

namespace HomeFlow.Features.Core.Tags;

public class UpdateTagsCommandValidator : AbstractValidator<UpdateTagsCommand>
{
    public UpdateTagsCommandValidator()
    {
        RuleFor( x => x.EntityTypeName )
            .NotEmpty().WithMessage( "Entity type name is required." )
            .MaximumLength( 100 ).WithMessage( "Entity type name cannot exceed 100 characters." );

        RuleFor( x => x.EntityId )
            .NotEmpty().WithMessage( "Entity ID is required." );

        RuleFor( x => x.Tags )
            .NotNull().WithMessage( "Tags list is required." );

        When( x => x.Tags != null, () =>
        {
            RuleForEach( x => x.Tags )
                .NotEmpty().WithMessage( "Tag cannot be empty." )
                .MaximumLength( 50 ).WithMessage( "Tag cannot exceed 50 characters." );
        } );
    }
}