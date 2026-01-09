

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class UpdateMealPlannerItemCommandValidator : AbstractValidator<UpdateMealPlannerItemCommand>
{
    public UpdateMealPlannerItemCommandValidator()
    {
        RuleFor( x => x.MealPlannerItem )
            .NotNull().WithMessage( "Meal planner item is required." );

        When( x => x.MealPlannerItem != null, () =>
        {
            RuleFor( x => x.MealPlannerItem.Id )
                .NotEmpty().WithMessage( "Meal planner item ID is required." );

            RuleFor( x => x.MealPlannerItem )
                .SetValidator( new MealPlannerItemValidator() );
        } );
    }
}