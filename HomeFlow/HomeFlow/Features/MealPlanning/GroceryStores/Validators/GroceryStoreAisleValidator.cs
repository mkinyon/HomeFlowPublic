namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreAisleValidator : AbstractValidator<GroceryStoreAisle>
{
    public GroceryStoreAisleValidator()
    {
        RuleFor( x => x.Name )
            .NotEmpty().WithMessage( "The aisle name is required." )
            .MaximumLength( 100 ).WithMessage( "The aisle name cannot exceed 100 characters." );

        RuleFor( x => x.Order )
            .GreaterThanOrEqualTo( 0 ).WithMessage( "Order cannot be negative." );
    }
} 