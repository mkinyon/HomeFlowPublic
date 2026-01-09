using HomeFlow.Interfaces;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeStepEntity : Model, IOrderable
{
    public Guid RecipeId { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Order { get; set; }

    public virtual RecipeEntity Recipe { get; set; } = default!;
}
