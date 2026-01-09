using System.ComponentModel.DataAnnotations;

namespace HomeFlow.Features.People.Contacts;

public class ContactValidator : AbstractValidator<Contact>
{
    public ContactValidator()
    {
        RuleFor( x => x.FirstName )
            .NotEmpty().WithMessage( "The first name is required." )
            .MaximumLength( 50 ).WithMessage( "The first name cannot exceed 50 characters." );

        RuleFor( x => x.LastName )
            .NotEmpty().WithMessage( "The last name is required." )
            .MaximumLength( 50 ).WithMessage( "The last name cannot exceed 50 characters." );

        RuleFor( x => x.Email )
            .Must( email => string.IsNullOrWhiteSpace( email ) || new EmailAddressAttribute().IsValid( email ) )
                .WithMessage( "The email must be a valid email address." )
            .MaximumLength( 100 ).WithMessage( "The email cannot exceed 100 characters." );

        RuleFor( x => x.PhoneNumber )
            .MaximumLength( 20 ).WithMessage( "The phone number cannot exceed 20 characters." );

        RuleFor( x => x.BirthDate )
            .LessThan( DateOnly.FromDateTime( DateTime.Today ) ).WithMessage( "The birth date cannot be in the future." );

        RuleFor( x => x.AnniversaryDate )
            .LessThan( DateOnly.FromDateTime( DateTime.Today ) ).WithMessage( "The anniversary date cannot be in the future." );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<Contact>.CreateWithOptions( (Contact) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
}
