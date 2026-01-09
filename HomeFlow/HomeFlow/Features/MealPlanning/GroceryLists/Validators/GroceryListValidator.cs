namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryListValidator : AbstractValidator<GroceryList>
{
    public GroceryListValidator()
    {
        RuleFor( x => x.Name )
            .NotEmpty().WithMessage( "The grocery list name is required." )
            .MaximumLength( 100 ).WithMessage( "The grocery list name cannot exceed 100 characters." );

        RuleForEach( x => x.Items )
            .SetValidator( new GroceryListItemValidator() );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<GroceryList>.CreateWithOptions( (GroceryList) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
} 