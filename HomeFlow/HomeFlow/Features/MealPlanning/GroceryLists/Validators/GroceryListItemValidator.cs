namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryListItemValidator : AbstractValidator<GroceryListItem>
{
    public GroceryListItemValidator()
    {
        RuleFor( x => x )
            .Must( item => item.GroceryItem != null || item.RecipeGroceryItem != null )
            .WithMessage( "Either a grocery item or a recipe grocery item must be provided." );

        RuleFor( x => x.AdditionalInfo )
            .MaximumLength( 200 ).WithMessage( "Additional info cannot exceed 200 characters." );
    }
}