namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class UpdateGroceryListCommandValidator : AbstractValidator<UpdateGroceryListCommand>
{
    public UpdateGroceryListCommandValidator()
    {
        RuleFor( x => x.GroceryList )
            .NotNull().WithMessage( "Grocery list is required." );

        When( x => x.GroceryList != null, () =>
        {
            RuleFor( x => x.GroceryList.Id )
                .NotEmpty().WithMessage( "Grocery list ID is required." );

            RuleFor( x => x.GroceryList )
                .SetValidator( new GroceryListValidator() );
        } );
    }
}