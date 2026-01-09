
using HomeFlow.Features.Core.ImageFiles;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class Recipe
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RecipeType RecipeType { get; set; }

    public int Servings { get; set; }

    public int PrepTimeInMinutes { get; set; }

    public int CookTimeInMinutes { get; set; }

    public int TotalTimeInMinutes { get; set; }

    public string Author { get; set; } = string.Empty;

    public ImageFile? Image { get; set; }

    public List<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();

    public List<RecipeGroceryItem> RecipeGroceryItems { get; set; } = new List<RecipeGroceryItem>();

    public List<string> Tags { get; set; } = new List<string>();

    public override string ToString() => Name;
}