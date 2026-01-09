namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class CreateGroceryItemCommandValidator : AbstractValidator<CreateGroceryItemCommand>
{
    public CreateGroceryItemCommandValidator()
    {
        RuleFor( x => x.GroceryItemRequest )
            .NotNull().WithMessage( "Grocery item request is required." );

        When( x => x.GroceryItemRequest != null, () =>
        {
            RuleFor( x => x.GroceryItemRequest )
                .SetValidator( new GroceryItemValidator() );
        } );
    }
}