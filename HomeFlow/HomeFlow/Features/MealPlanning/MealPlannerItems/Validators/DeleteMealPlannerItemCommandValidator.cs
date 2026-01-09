

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class DeleteMealPlannerItemCommandValidator : AbstractValidator<DeleteMealPlannerItemCommand>
{
    public DeleteMealPlannerItemCommandValidator()
    {
        RuleFor( x => x.Id )
            .NotEmpty().WithMessage( "Meal planner item ID is required." );
    }
}