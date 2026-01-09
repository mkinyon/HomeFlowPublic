using HomeFlow.Features.MealPlanning.GroceryLists;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryLists
{
    [Inject]
    IGroceryListService GroceryListService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private MudTable<GroceryList> table = default!;
    private MudMessageBox _mudMessageBox = new MudMessageBox();
    private bool _includeCompleted;

    private async Task ToggleCompleted()
    {
        _includeCompleted = !_includeCompleted;

        await table.ReloadServerData();
        StateHasChanged();
    }

    private void ViewGroceryList( TableRowClickEventArgs<GroceryList> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"/grocerylist/{args.Item.Id}" );
        }
    }

    private void NewGroceryList()
    {
        NavigationManager.NavigateTo( "/grocerylist" );
    }

    private async Task DeleteGroceryListAsync( Guid id )
    {
        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if ( state == "Deleted!" )
        {
            await GroceryListService.DeleteAsync( id );
        }

        await table.ReloadServerData();
        StateHasChanged();
    }

    private async Task<MudBlazor.TableData<GroceryList>> GroceryListData( TableState state, CancellationToken cancellationToken )
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        queryOptions.AdditionalSettings.Add( "IncludeCompleted", _includeCompleted.ToString() );

        var response = await GroceryListService.GetTableDataAsync( queryOptions );

        return new MudBlazor.TableData<GroceryList>
        {
            Items = response.Items,
            TotalItems = response.TotalItems
        };
    }
}
