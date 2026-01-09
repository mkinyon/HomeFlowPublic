namespace HomeFlow.Features.People.Contacts;

public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "The contact ID is required." );
    }
}
