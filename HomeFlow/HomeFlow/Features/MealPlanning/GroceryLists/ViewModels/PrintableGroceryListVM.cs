namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class PrintableGroceryListVM
{
    public string Name { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string StoreLocation { get; set; } = string.Empty;

    public List<PrintableGroceryCategoryVM> Categories { get; set; } = new List<PrintableGroceryCategoryVM>();
}

public class PrintableGroceryCategoryVM
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<PrintableGroceryItemVM> GroceryItems { get; set; } = new List<PrintableGroceryItemVM>();
}

public class PrintableGroceryItemVM
{
    public string Name { get; set; } = string.Empty;
}
