
namespace HomeFlow.Features.MealPlanning.Recipes;

public class AIRecipe
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PrepTimeInMinutes { get; set; }

    public int CookTimeInMinutes { get; set; }

    public int TotalTimeInMinutes { get; set; }

    public string Author { get; set; } = string.Empty;

    public List<AIRecipeStep> RecipeSteps { get; set; } = new List<AIRecipeStep>();

    public List<AIRecipeGroceryItem> RecipeGroceryItems { get; set; } = new List<AIRecipeGroceryItem>();

    public List<string> Tags { get; set; } = new List<string>();
}
