

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
        RuleFor( x => x.Name )
            .NotEmpty().WithMessage( "The recipe name is required." )
            .MaximumLength( 100 ).WithMessage( "The recipe name cannot exceed 100 characters." );

        RuleForEach( x => x.RecipeGroceryItems )
            .SetValidator( new RecipeGroceryItemValidator() );

        RuleForEach( x => x.RecipeSteps )
            .ChildRules( step =>
            {
                step.RuleFor( s => s.Text )
                    .NotEmpty().WithMessage( "Recipe step text is required." )
                    .MaximumLength( 500 ).WithMessage( "Recipe step text cannot exceed 500 characters." );
            } );

        RuleFor( x => x.Servings )
            .GreaterThan( 0 ).WithMessage( "Servings must be greater than 0." );

        RuleFor( x => x.PrepTimeInMinutes )
            .GreaterThanOrEqualTo( 0 ).WithMessage( "Prep time cannot be negative." );

        RuleFor( x => x.CookTimeInMinutes )
            .GreaterThanOrEqualTo( 0 ).WithMessage( "Cook time cannot be negative." );

        RuleFor( x => x.TotalTimeInMinutes )
            .GreaterThanOrEqualTo( 0 ).WithMessage( "Total time cannot be negative." );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<Recipe>.CreateWithOptions( (Recipe) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
}
