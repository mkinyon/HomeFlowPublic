
namespace HomeFlow.Features.MealPlanning;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<Features.MealPlanning.Recipes.Recipe, Features.MealPlanning.Recipes.RecipeEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.Recipes.RecipeStep, Features.MealPlanning.Recipes.RecipeStepEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.Recipes.RecipeGroceryItem, Features.MealPlanning.Recipes.RecipeGroceryItemEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.GroceryItems.GroceryItem, Features.MealPlanning.GroceryItems.GroceryItemEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.GroceryStores.GroceryStore, Features.MealPlanning.GroceryStores.GroceryStoreEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.GroceryStores.GroceryStoreAisle, Features.MealPlanning.GroceryStores.GroceryStoreAisleEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.GroceryLists.GroceryList, Features.MealPlanning.GroceryLists.GroceryListEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.GroceryLists.GroceryListItem, Features.MealPlanning.GroceryLists.GroceryListItemEntity>().ReverseMap();
        CreateMap<Features.MealPlanning.MealPlannerItems.MealPlannerItem, Features.MealPlanning.MealPlannerItems.MealPlannerItemEntity>().ReverseMap();

        CreateMap<Features.MealPlanning.Recipes.AIRecipe, Features.MealPlanning.Recipes.Recipe>().ReverseMap();
        CreateMap<Features.MealPlanning.Recipes.AIRecipeStep, Features.MealPlanning.Recipes.RecipeStep>().ReverseMap();
        CreateMap<Features.MealPlanning.Recipes.AIRecipeGroceryItem, Features.MealPlanning.Recipes.RecipeGroceryItem>();
        CreateMap<Features.MealPlanning.Recipes.AIGroceryItem, Features.MealPlanning.GroceryItems.GroceryItem>();
    }
}
