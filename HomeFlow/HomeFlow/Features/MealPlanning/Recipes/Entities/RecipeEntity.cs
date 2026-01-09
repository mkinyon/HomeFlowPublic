using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public RecipeType RecipeType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PrepTimeInMinutes { get; set; }

    public int CookTimeInMinutes { get; set; }

    public int TotalTimeInMinutes { get; set; }

    public Guid? ImageId { get; set; }

    public virtual List<RecipeStepEntity> RecipeSteps { get; set; } = new List<RecipeStepEntity>();

    public virtual List<RecipeGroceryItemEntity> RecipeGroceryItems { get; set; } = new List<RecipeGroceryItemEntity>();

    public virtual ImageFileEntity? Image { get; set; }
}
