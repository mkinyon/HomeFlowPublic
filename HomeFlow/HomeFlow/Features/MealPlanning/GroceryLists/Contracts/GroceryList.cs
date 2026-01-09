namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrinted { get; set; }
    public virtual ICollection<GroceryListItem> Items { get; set; } = new List<GroceryListItem>();

    public override string ToString() => Name;
}
