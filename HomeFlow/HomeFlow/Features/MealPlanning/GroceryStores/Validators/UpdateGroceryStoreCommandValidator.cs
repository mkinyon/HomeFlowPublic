namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class UpdateGroceryStoreCommandValidator : AbstractValidator<UpdateGroceryStoreCommand>
{
    public UpdateGroceryStoreCommandValidator()
    {
        RuleFor( x => x.GroceryStore )
            .NotNull().WithMessage( "Grocery store is required." );

        When( x => x.GroceryStore != null, () =>
        {
            RuleFor( x => x.GroceryStore.Id )
                .NotEmpty().WithMessage( "Grocery store ID is required." );

            RuleFor( x => x.GroceryStore )
                .SetValidator( new GroceryStoreValidator() );
        } );
    }
}