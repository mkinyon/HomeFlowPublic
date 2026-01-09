namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class DeleteGroceryStoreCommandValidator : AbstractValidator<DeleteGroceryStoreCommand>
{
    public DeleteGroceryStoreCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Grocery store ID is required." );
    }
}