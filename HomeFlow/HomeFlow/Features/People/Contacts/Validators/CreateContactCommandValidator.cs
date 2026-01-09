using System.ComponentModel.DataAnnotations;

namespace HomeFlow.Features.People.Contacts;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor( x => x.Contact )
            .NotNull().WithMessage( "Contact is required." );

        When( x => x.Contact != null, () =>
        {
            RuleFor( x => x.Contact.FirstName )
                .NotEmpty().WithMessage( "The first name is required." )
                .MaximumLength( 50 ).WithMessage( "The first name cannot exceed 50 characters." );

            RuleFor( x => x.Contact.LastName )
                .NotEmpty().WithMessage( "The last name is required." )
                .MaximumLength( 50 ).WithMessage( "The last name cannot exceed 50 characters." );

            RuleFor( x => x.Contact.Email )
            .Must( email => string.IsNullOrWhiteSpace( email ) || new EmailAddressAttribute().IsValid( email ) )
                .WithMessage( "The email must be a valid email address." )
            .MaximumLength( 100 ).WithMessage( "The email cannot exceed 100 characters." );

            RuleFor( x => x.Contact.PhoneNumber )
                .MaximumLength( 20 ).WithMessage( "The phone number cannot exceed 20 characters." );

            RuleFor( x => x.Contact.BirthDate )
                .LessThan( DateOnly.FromDateTime( DateTime.Today ) ).WithMessage( "The birth date cannot be in the future." );

            RuleFor( x => x.Contact.AnniversaryDate )
                .LessThan( DateOnly.FromDateTime( DateTime.Today ) ).WithMessage( "The anniversary date cannot be in the future." );
        } );
    }
}
