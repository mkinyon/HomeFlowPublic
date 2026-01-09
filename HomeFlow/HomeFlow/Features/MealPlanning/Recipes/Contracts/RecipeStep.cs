
using HomeFlow.Interfaces;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeStep : IOrderable
{
    public Guid Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Order { get; set; }

    public override string ToString() => Text;
}
