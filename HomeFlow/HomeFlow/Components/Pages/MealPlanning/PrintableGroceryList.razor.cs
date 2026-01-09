
using HomeFlow.Features.MealPlanning.GroceryLists;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class PrintableGroceryList
{
    [Inject]
    IGroceryListService GroceryListService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    [Parameter]
    public Guid? GroceryStoreId { get; set; }

    private PrintableGroceryListVM _groceryList = new();

    private List<PrintableGroceryCategoryVM> _columnOneAisles = new();
    private List<PrintableGroceryCategoryVM> _columnTwoAisles = new();

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            _groceryList = await GroceryListService.GetPrintableListAsync( Id.Value, GroceryStoreId );

            // Split the aisles into two even columns
            int aisleCount = _groceryList.Categories.Count;

            if ( aisleCount > 1 )
            {
                int midPoint = aisleCount / 2;
                _columnOneAisles = _groceryList.Categories.Take( midPoint ).ToList();
                _columnTwoAisles = _groceryList.Categories.Skip( midPoint ).ToList();
            }
            else
            {
                _columnOneAisles = _groceryList.Categories.ToList();
            }
        }
    }
}
