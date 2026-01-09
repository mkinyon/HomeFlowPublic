

namespace HomeFlow.Features.MealPlanning.Recipes;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor( x => x.RecipeRequest )
            .NotNull().WithMessage( "Recipe request is required." );

        When( x => x.RecipeRequest != null, () =>
        {
            RuleFor( x => x.RecipeRequest )
                .SetValidator( new RecipeValidator() );
        } );
    }
}