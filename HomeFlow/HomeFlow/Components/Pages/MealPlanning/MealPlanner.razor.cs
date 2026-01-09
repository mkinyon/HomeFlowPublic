using Heron.MudCalendar;
using HomeFlow.Features.MealPlanning.GroceryLists;
using HomeFlow.Features.MealPlanning.MealPlannerItems;
using HomeFlow.Features.MealPlanning.Recipes;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class MealPlanner
{
    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Inject]
    IMealPlannerItemService MealPlannerService { get; set; } = default!;

    [Inject]
    IGroceryListService GroceryListService { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    public bool ItemsSelected
    {
        get => _recipeDropItems.Any( i => i.Selected );
    }

    private List<RecipeDropItem> _recipeDropItems = new();
    private List<GroceryList> _groceryLists = new();
    private MudForm form = new MudForm();
    private DateRange selectedDateRange = new DateRange();
    private MudMessageBox _mudMessageBox = new MudMessageBox();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var startOfMonth = new DateTime( DateTime.Now.Year, DateTime.Now.Month, 1 );
        var daysInMonth = DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month );
        var lastDayOfMonth = new DateTime( DateTime.Now.Year, DateTime.Now.Month, daysInMonth );

        selectedDateRange = new DateRange( startOfMonth, lastDayOfMonth );

        var options = new QueryOptions
        {
            SearchTerm = string.Empty,
            SortBy = "created",
            SortDescending = true,
            PageSize = 10,
            Page = 1
        };

        var data = await GroceryListService.GetTableDataAsync( options );
        _groceryLists = data.Items;

        _isLoading = false;
    }

    private Color GetButtonColor( RecipeDropItem item )
    {
        switch ( item.RecipeType )
        {
            case RecipeType.MainDish:
                return Color.Primary;
            case RecipeType.SideDish:
                return Color.Secondary;
            default:
                return Color.Default;
        }
    }

    private Variant GetButtonVariant( RecipeDropItem item )
    {
        return item.Selected ? Variant.Filled : Variant.Outlined;
    }

    public async Task GetRecipesDropItemsAsync( DateRange dateRange )
    {
        _recipeDropItems.Clear();

        var mealPlannerItems = await MealPlannerService.GetByDateRange( dateRange.Start, dateRange.End );

        var mealPlannerDropItems = mealPlannerItems
            .Select( mpi => new RecipeDropItem
            {
                MealPlannerItemId = mpi.MealPlannerItemId,
                ImageUrl = mpi.ImageUrl,
                RecipeId = mpi.RecipeId,
                RecipeType = mpi.RecipeType,
                Text = mpi.RecipeName,
                Start = mpi.Date.ToDateTime( TimeOnly.MinValue ),
                End = mpi.Date.ToDateTime( TimeOnly.MaxValue )
            } ).ToList();

        _recipeDropItems.AddRange( mealPlannerDropItems );
    }

    private async Task AddRecipesAsync( Guid groceryListId )
    {
        foreach ( var item in _recipeDropItems.Where( i => i.Selected ) )
        {
            var recipe = await RecipeService.GetByIdAsync( item.RecipeId );
            if ( recipe == null )
            {
                Snackbar.Add( "Recipe not found.", MudBlazor.Severity.Error );
                continue;
            }

            foreach ( var recipeGroceryItem in recipe.RecipeGroceryItems )
            {
                var itemRequest = new GroceryListItem
                {
                    SourceRecipe = recipe,
                    RecipeGroceryItem = recipeGroceryItem,
                };

                await GroceryListService.AddItemAsync( groceryListId, itemRequest );
            }

            item.Selected = false;
        }

        Snackbar.Add( "Recipes added to grocery list.", MudBlazor.Severity.Success );
    }

    #region Calendar Events

    private async Task AddRecipe( DateTime dateTime )
    {
        await OpenDialog( dateTime );
    }

    private void ItemChanged( CalendarItem calendarItem )
    {
        var recipeDropItem = calendarItem as RecipeDropItem;
        if ( recipeDropItem != null )
        {
            MealPlannerService.UpdateAsync( new MealPlannerItem
            {
                Id = recipeDropItem.MealPlannerItemId,
                Date = DateOnly.FromDateTime( recipeDropItem.Start ),
                RecipeId = recipeDropItem.RecipeId
            } );
        }
    }

    private async Task DateRangeChangedAsync( DateRange dateRange )
    {
        await GetRecipesDropItemsAsync( dateRange );
        selectedDateRange = dateRange;
    }

    #endregion

    #region Add Dialog

    private async Task OpenDialog( DateTime dateTime )
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<RecipeFinderDialog>( "Add Recipe", new DialogParameters<RecipeFinderDialog>(), options );

        var result = await dialog.Result;

        if ( result != null && !result.Canceled && result.Data is Recipe )
        {
            var selectedRecipe = result.Data as Recipe;

            MealPlannerItem mealPlannerItem = new MealPlannerItem
            {
                Date = DateOnly.FromDateTime( dateTime ),
                RecipeId = selectedRecipe!.Id
            };

            await MealPlannerService.CreateAsync( mealPlannerItem );
            await GetRecipesDropItemsAsync( selectedDateRange );

            StateHasChanged();
        }
    }

    #endregion

    #region Delete Dialog

    private async Task DeleteItemsAsync()
    {
        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if ( state == "Deleted!" )
        {
            foreach ( var item in _recipeDropItems.Where( i => i.Selected ).ToList() )
            {
                await MealPlannerService.DeleteAsync( item.MealPlannerItemId );
            }
        }

        await GetRecipesDropItemsAsync( selectedDateRange );
        Snackbar.Add( "Recipes removed.", MudBlazor.Severity.Success );
        StateHasChanged();
    }

    private void OpenAddDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Large
        };
    }

    #endregion

    public class RecipeDropItem : CalendarItem
    {
        public bool Selected { get; set; }
        public Guid MealPlannerItemId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public Guid RecipeId { get; set; }
        public RecipeType RecipeType { get; set; }
    }
}