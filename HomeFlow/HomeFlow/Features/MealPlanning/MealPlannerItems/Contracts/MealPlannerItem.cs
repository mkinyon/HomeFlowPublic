namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class MealPlannerItem
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public Guid RecipeId { get; set; }
}
