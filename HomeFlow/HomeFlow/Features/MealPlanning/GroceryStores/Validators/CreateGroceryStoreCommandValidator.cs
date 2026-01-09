namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class CreateGroceryStoreCommandValidator : AbstractValidator<CreateGroceryStoreCommand>
{
    public CreateGroceryStoreCommandValidator()
    {
        RuleFor( x => x.GroceryStoreRequest )
            .NotNull().WithMessage( "Grocery store request is required." );

        When( x => x.GroceryStoreRequest != null, () =>
        {
            RuleFor( x => x.GroceryStoreRequest )
                .SetValidator( new GroceryStoreValidator() );
        } );
    }
}