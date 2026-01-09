using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.Recipes;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryListItem
{
    public Guid Id { get; set; }
    public Recipe? SourceRecipe { get; set; }
    public RecipeGroceryItem? RecipeGroceryItem { get; set; }
    public GroceryItem? GroceryItem { get; set; }
    public int Quantity { get; set; }
    public string AdditionalInfo { get; set; } = string.Empty;
    public int Order { get; set; }

    public string Text
    {
        get
        {
            string result = string.Empty;

            if ( RecipeGroceryItem != null )
            {
                result = RecipeGroceryItem.ToString() + (AdditionalInfo != string.Empty ? " (" + AdditionalInfo + ")" : string.Empty);
            }
            else if ( GroceryItem != null )
            {
                result = GroceryItem.Name + (AdditionalInfo != string.Empty ? " (" + AdditionalInfo + ")" : string.Empty);
                if ( Quantity > 1 )
                {
                    result = $"{Quantity} {result}";
                }
            }

            return result;
        }
    }

    public override string ToString()
    {
        return Text;
    }
}
