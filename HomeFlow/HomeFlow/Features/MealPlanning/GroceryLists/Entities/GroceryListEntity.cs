using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryListEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsPrinted { get; set; }

    public virtual ICollection<GroceryListItemEntity> Items { get; set; } = new List<GroceryListItemEntity>();

    public override string ToString()
    {
        return Name;
    }
}
