
namespace HomeFlow.Features.MealPlanning.Recipes;

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor( x => x.Recipe )
            .NotNull().WithMessage( "Recipe is required." );

        When( x => x.Recipe != null, () =>
        {
            RuleFor( x => x.Recipe.Id )
                .NotEmpty().WithMessage( "Recipe ID is required." );

            RuleFor( x => x.Recipe )
                .SetValidator( new RecipeValidator() );

            // Additional validation specific to updates
            RuleFor( x => x.Recipe.RecipeGroceryItems )
                .NotEmpty().WithMessage( "The recipe must contain at least one grocery item." );

            RuleFor( x => x.Recipe.RecipeSteps )
                .NotEmpty().WithMessage( "The recipe must contain at least one step." );
        } );
    }
}