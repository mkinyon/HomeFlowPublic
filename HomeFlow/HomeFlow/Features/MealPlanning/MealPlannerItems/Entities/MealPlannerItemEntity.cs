using HomeFlow.Features.MealPlanning.Recipes;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public class MealPlannerItemEntity : Model
{
    public DateOnly Date { get; set; }

    public Guid RecipeId { get; set; }

    public virtual RecipeEntity Recipe { get; set; } = default!;
}
