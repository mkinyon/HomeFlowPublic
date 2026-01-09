namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class AddGroceryListItemCommandValidator : AbstractValidator<AddGroceryListItemCommand>
{
    public AddGroceryListItemCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Grocery list ID is required." );

        RuleFor( x => x.GroceryListItemRequest )
            .NotNull().WithMessage( "Grocery list item request is required." );

        When( x => x.GroceryListItemRequest != null, () =>
        {
            RuleFor( x => x.GroceryListItemRequest )
                .SetValidator( new GroceryListItemValidator() );
        } );
    }
}