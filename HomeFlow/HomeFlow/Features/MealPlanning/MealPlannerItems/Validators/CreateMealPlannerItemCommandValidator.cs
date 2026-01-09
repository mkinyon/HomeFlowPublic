
namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class CreateMealPlannerItemCommandValidator : AbstractValidator<CreateMealPlannerItemCommand>
{
    public CreateMealPlannerItemCommandValidator()
    {
        RuleFor( x => x.MealPlannerItem )
            .NotNull().WithMessage( "Meal planner item is required." );

        When( x => x.MealPlannerItem != null, () =>
        {
            RuleFor( x => x.MealPlannerItem )
                .SetValidator( new MealPlannerItemValidator() );
        } );
    }
}