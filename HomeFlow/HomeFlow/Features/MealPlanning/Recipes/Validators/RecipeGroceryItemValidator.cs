

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeGroceryItemValidator : AbstractValidator<RecipeGroceryItem>
{
    public RecipeGroceryItemValidator()
    {
        RuleFor( x => x.GroceryItem )
            .NotNull().WithMessage( "All recipe grocery items must have an assigned Grocery Item." );

        When( x => x.GroceryItem != null, () =>
        {
            RuleFor( x => x.GroceryItem.Name )
                .NotEmpty().WithMessage( "Grocery item name is required." );
        } );

        RuleFor( x => x.AdditionalDetail )
            .MaximumLength( 200 ).WithMessage( "Additional detail cannot exceed 200 characters." );
    }
}