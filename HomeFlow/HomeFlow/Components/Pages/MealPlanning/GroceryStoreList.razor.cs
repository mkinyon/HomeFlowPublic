using HomeFlow.Features.MealPlanning.GroceryStores;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryStoreList
{
    [Inject]
    IGroceryStoreService GroceryStoreService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private ICollection<GroceryStore> _groceryStores = new List<GroceryStore>();

    private MudTable<GroceryStore> table = default!;
    private MudMessageBox _mudMessageBox = new MudMessageBox();
    private string _errorMessage = string.Empty;
    private string _searchString = "";

    private void ViewGroceryStore( TableRowClickEventArgs<GroceryStore> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"/grocerystore/{args.Item.Id}" );
        }
    }

    private void NewGroceryStore()
    {
        NavigationManager.NavigateTo( "/grocerystore" );
    }

    private async Task DeleteGroceryListAsync( Guid id )
    {
        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if ( state == "Deleted!" )
        {
            await GroceryStoreService.DeleteAsync( id );
            _groceryStores!.Remove( _groceryStores.First( i => i.Id == id ) );
        }

        await table.ReloadServerData();
        StateHasChanged();
    }

    private async Task<MudBlazor.TableData<GroceryStore>> GroceryListData( TableState state, CancellationToken cancellationToken )
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchTerm = _searchString,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        var response = await GroceryStoreService.GetTableDataAsync( queryOptions );

        return new MudBlazor.TableData<GroceryStore>
        {
            Items = response.Items,
            TotalItems = response.TotalItems
        };
    }

    private void SearchAsync()
    {
        table.ReloadServerData();
    }
}
