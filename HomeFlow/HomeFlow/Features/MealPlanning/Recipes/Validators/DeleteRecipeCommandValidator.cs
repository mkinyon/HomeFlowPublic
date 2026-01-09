

namespace HomeFlow.Features.MealPlanning.Recipes;

public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    public DeleteRecipeCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Recipe ID is required." );
    }
}