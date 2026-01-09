namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class DeleteGroceryItemCommandValidator : AbstractValidator<DeleteGroceryItemCommand>
{
    public DeleteGroceryItemCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Grocery item ID is required." );
    }
}