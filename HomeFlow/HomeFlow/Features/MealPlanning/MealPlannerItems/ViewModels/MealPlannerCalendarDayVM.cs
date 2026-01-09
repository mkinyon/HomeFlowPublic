

using HomeFlow.Features.MealPlanning.Recipes;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class MealPlannerCalendarDayVM
{
    public Guid MealPlannerItemId { get; init; }

    public DateOnly Date { get; init; }

    public Guid RecipeId { get; init; }

    public RecipeType RecipeType { get; init; }

    public string RecipeName { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = string.Empty;

    public bool InGroceryList { get; init; }
}
