

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class UpdateGroceryItemCommandValidator : AbstractValidator<UpdateGroceryItemCommand>
{
    public UpdateGroceryItemCommandValidator()
    {
        RuleFor( x => x.GroceryItem )
            .NotNull().WithMessage( "Grocery item is required." );

        When( x => x.GroceryItem != null, () =>
        {
            RuleFor( x => x.GroceryItem.Id )
                .NotEmpty().WithMessage( "Grocery item ID is required." );

            RuleFor( x => x.GroceryItem )
                .SetValidator( new GroceryItemValidator() );
        } );
    }
}