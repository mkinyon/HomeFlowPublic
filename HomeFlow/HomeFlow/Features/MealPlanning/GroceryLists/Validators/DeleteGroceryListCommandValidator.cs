namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class DeleteGroceryListCommandValidator : AbstractValidator<DeleteGroceryListCommand>
{
    public DeleteGroceryListCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Grocery list ID is required." );
    }
}