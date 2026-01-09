namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class CreateGroceryListCommandValidator : AbstractValidator<CreateGroceryListCommand>
{
    public CreateGroceryListCommandValidator()
    {
        RuleFor( x => x.GroceryListRequest )
            .NotNull().WithMessage( "Grocery list request is required." );

        When( x => x.GroceryListRequest != null, () =>
        {
            RuleFor( x => x.GroceryListRequest )
                .SetValidator( new GroceryListValidator() );
        } );
    }
}