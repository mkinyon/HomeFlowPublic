using FluentValidation;

namespace HomeFlow.Features.Core.ImageFiles;

public class UpdateImageFileCommandValidator : AbstractValidator<UpdateImageFileCommand>
{
    public UpdateImageFileCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Image file ID is required." );

        RuleFor( x => x.Request )
            .NotNull().WithMessage( "Image file request is required." );

        When( x => x.Request != null, () =>
        {
            RuleFor( x => x.Request.Data )
                .NotNull().WithMessage( "File data is required." )
                .Must( data => data != null && data.Length > 0 ).WithMessage( "File data cannot be empty." )
                .Must( data => data != null && data.Length <= 10 * 1024 * 1024 ).WithMessage( "File size cannot exceed 10MB." );

            RuleFor( x => x.Request.Width )
                .GreaterThan( 0 ).WithMessage( "Width must be greater than 0." );

            RuleFor( x => x.Request.Height )
                .GreaterThan( 0 ).WithMessage( "Height must be greater than 0." );
        } );
    }
}
