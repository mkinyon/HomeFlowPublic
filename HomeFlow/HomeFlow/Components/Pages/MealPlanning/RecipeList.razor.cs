
using HomeFlow.Features.MealPlanning.Recipes;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace HomeFlow.Components.Pages.MealPlanning;

public partial class RecipeList
{
    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private MudTable<Recipe> table = default!;
    private string _searchString = "";

    private void ViewRecipe( TableRowClickEventArgs<Recipe> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"/recipe/{args.Item.Id}" );
        }
    }

    private async Task<MudBlazor.TableData<Recipe>> RecipeData( TableState state, CancellationToken cancellationToken )
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchTerm = _searchString,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        var response = await RecipeService.GetTableDataAsync( queryOptions );

        return new MudBlazor.TableData<Recipe>
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