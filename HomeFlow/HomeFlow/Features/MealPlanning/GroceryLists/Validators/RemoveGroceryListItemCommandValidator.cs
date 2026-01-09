namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class RemoveGroceryListItemCommandValidator : AbstractValidator<RemoveGroceryListItemCommand>
{
    public RemoveGroceryListItemCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Grocery list item ID is required." );
    }
}