namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreValidator : AbstractValidator<GroceryStore>
{
    public GroceryStoreValidator()
    {
        RuleFor( x => x.Name )
            .NotEmpty().WithMessage( "The grocery store name is required." )
            .MaximumLength( 100 ).WithMessage( "The grocery store name cannot exceed 100 characters." );

        RuleFor( x => x.Location )
            .MaximumLength( 200 ).WithMessage( "The location cannot exceed 200 characters." );

        RuleForEach( x => x.GroceryStoreAisles )
            .SetValidator( new GroceryStoreAisleValidator() );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<GroceryStore>.CreateWithOptions( (GroceryStore) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
} 