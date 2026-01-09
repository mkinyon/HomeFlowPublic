
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryItemList
{
    [Inject]
    ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    IGroceryItemService GroceryItemService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private MudTable<GroceryItemVM> table = default!;
    private MudMessageBox _mudMessageBox = new MudMessageBox();
    private string _searchString = "";

    private HashSet<GroceryItemVM> _selectedItems = new HashSet<GroceryItemVM>();

    private async Task OnDeleteClickAsync( Guid id )
    {
        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if ( state == "Deleted!" )
        {
            await GroceryItemService.DeleteAsync( id );
        }

        await table.ReloadServerData();
        StateHasChanged();
    }

    private void Add()
    {
        NavigationManager.NavigateTo( "groceryitem" );
    }

    private void ViewGroceryList( TableRowClickEventArgs<GroceryItemVM> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"/groceryitem/{args.Item.Id}" );
        }
    }

    private async Task<MudBlazor.TableData<GroceryItemVM>> GroceryItemVMData( TableState state, CancellationToken cancellationToken )
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchTerm = _searchString,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        var response = await GroceryItemService.GetVMTableDataAsync( queryOptions );

        return new MudBlazor.TableData<GroceryItemVM>
        {
            Items = response.Items,
            TotalItems = response.TotalItems
        };
    }

    private void SearchAsync()
    {
        table.ReloadServerData();
    }

    private async Task MergeGroceryItems()
    {
        var parameters = new DialogParameters<GroceryItemMerge>
        {
            { x => x.GroceryItemVMs, _selectedItems }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var task = await DialogService.ShowAsync<GroceryItemMerge>( "Dialog", parameters, options );

        var result = await task.Result;
        if ( result != null && !result.Canceled )
        {
            SnackBar.Add( $"Grocery items merged." );
            await table.ReloadServerData();
        }
    }
}
