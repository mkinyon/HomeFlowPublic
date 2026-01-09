namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class MealPlannerItemValidator : AbstractValidator<MealPlannerItem>
{
    public MealPlannerItemValidator()
    {
        RuleFor( x => x.Date )
            .NotEmpty().WithMessage( "Date is required." );

        RuleFor( x => x.RecipeId )
            .NotEmpty().WithMessage( "Recipe ID is required." );
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async ( model, propertyName ) =>
    {
        var result = await ValidateAsync( ValidationContext<MealPlannerItem>.CreateWithOptions( (MealPlannerItem) model, x => x.IncludeProperties( propertyName ) ) );
        if ( result.IsValid )
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select( e => e.ErrorMessage );
    };
} 