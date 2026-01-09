using HomeFlow.Features.Tasks.TodoLists;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.Tasks;

public partial class TodoLists
{
    [Inject]
    ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    ITodoListsService TodoListsService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private MudTable<TodoListVM> table = default!;
    private MudMessageBox _mudMessageBox = new MudMessageBox();
    private string _searchString = "";

    private async Task OnDeleteClickAsync(Guid id)
    {
        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if (state == "Deleted!")
        {
            await TodoListsService.DeleteAsync(id);
        }

        await table.ReloadServerData();
        StateHasChanged();
    }

    private void Add()
    {
        NavigationManager.NavigateTo("todolist");
    }

    private void ViewTodoList(TableRowClickEventArgs<TodoListVM> args)
    {
        if (args.Item != null)
        {
            NavigationManager.NavigateTo($"/todolist/{args.Item.Id}");
        }
    }

    private async Task<MudBlazor.TableData<TodoListVM>> TodoListVMData(TableState state, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchTerm = _searchString,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        var response = await TodoListsService.GetVMTableDataAsync(queryOptions);

        return new MudBlazor.TableData<TodoListVM>
        {
            Items = response.Items,
            TotalItems = response.TotalItems
        };
    }

    private void SearchAsync()
    {
        table.ReloadServerData();
    }

    /// <summary>
    /// Returns the appropriate color for the progress bar based on completion percentage
    /// </summary>
    /// <param name="completionPercentage">The completion percentage (0-100)</param>
    /// <returns>Color for the progress bar</returns>
    private Color GetProgressColor(double completionPercentage)
    {
        return completionPercentage switch
        {
            < 20 => Color.Error,      // Danger for less than 20%
            >= 20 and <= 80 => Color.Warning,  // Warning for 21-80%
            > 80 => Color.Success,    // Success for greater than 80%
            _ => Color.Default
        };
    }
}
