namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class GroceryItemValidator : AbstractValidator<GroceryItem>
{
    public GroceryItemValidator()
    {
        RuleFor( x => x.Name )
            .NotEmpty().WithMessage( "The grocery item name is required." )
            .MaximumLength( 100 ).WithMessage( "The grocery item name cannot exceed 100 characters." );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<GroceryItem>.CreateWithOptions( (GroceryItem) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
}
