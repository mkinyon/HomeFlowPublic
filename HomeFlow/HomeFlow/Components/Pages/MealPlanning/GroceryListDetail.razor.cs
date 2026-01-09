
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.GroceryLists;
using HomeFlow.Features.MealPlanning.GroceryStores;
using HomeFlow.Features.MealPlanning.Recipes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryListDetail
{
    [Inject]
    ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    IGroceryListService GroceryListService { get; set; } = default!;

    [Inject]
    IGroceryItemService GroceryItemService { get; set; } = default!;

    [Inject]
    IGroceryStoreService GroceryStoreService { get; set; } = default!;

    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    public GroceryList? GroceryList { get; set; }

    private string _searchTerm = string.Empty;

    private GroceryStore? _groceryStore;
    private List<GroceryItem> _groceryItemsAtStore = new();

    /// <summary>
    /// Initializes the component. If an Id is provided, it fetches the existing grocery list.
    /// Otherwise, it creates a new grocery list with the current date in the name.
    /// Also fetches the available recipes for the drop-down list.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue && Id.Value != Guid.Empty )
        {
            GroceryList = await GroceryListService.GetByIdAsync( Id.Value );
            GroceryList!.Items = GroceryList!.Items.OrderBy( i => i.SourceRecipe?.Name ?? "zz" ).ToList();
        }
        else
        {
            var today = DateTime.Now;
            GroceryList = new GroceryList { Name = $"Grocery List ({today.ToString( "MMM dd, yyyy" )})" };

            var groceryListId = await GroceryListService.CreateAsync( GroceryList );
            GroceryList.Id = groceryListId;
            NavigationManager.NavigateTo( $"/grocerylist/{groceryListId}" );
        }

        // get recipes
        _recipeDropItems = await GetRecipesDropItemsAsync();
    }

    /// <summary>
    /// Updates the grocery list with the selected recipe items.
    /// </summary>
    /// <param name="dropItem">The dropped recipe item.</param>
    private async void ItemUpdatedAsync( MudItemDropInfo<RecipeDropItem> dropItem )
    {
        if ( dropItem.DropzoneIdentifier == "Grocery List" )
        {
            var recipeId = dropItem.Item!.Id;
            var recipe = await RecipeService.GetByIdAsync( recipeId );

            if ( recipe == null )
            {
                SnackBar.Add( "Recipe not found.", MudBlazor.Severity.Error );
                return;
            }

            foreach ( var recipeGroceryItem in recipe.RecipeGroceryItems )
            {
                GroceryListItem groceryListItem = new GroceryListItem
                {
                    SourceRecipe = recipe,
                    RecipeGroceryItem = recipeGroceryItem
                };

                groceryListItem.Id = await GroceryListService.AddItemAsync( GroceryList!.Id, groceryListItem );
                GroceryList!.Items.Add( groceryListItem );
            }

            GroceryList!.Items = GroceryList!.Items.OrderBy( i => i.SourceRecipe?.Name ?? "zz" ).ToList();

            SnackBar.Add( $"Recipe \"{recipe.Name}\" Added", MudBlazor.Severity.Normal, config => { config.Icon = Icons.Material.Filled.Fastfood; } );
        }

        _recipeDropItems = await GetRecipesDropItemsAsync();
        StateHasChanged();
    }

    private async void SaveAdditionalInfo( object element )
    {
        var groceryListItem = (GroceryListItem) element;

        if ( GroceryList != null )
        {
            await GroceryListService.RemoveItemAsync( groceryListItem.Id );
            await GroceryListService.AddItemAsync( GroceryList.Id, groceryListItem );
            StateHasChanged();
        }
    }

    private async void UpdateGroceryListName()
    {
        if ( GroceryList != null && GroceryList.Id != Guid.Empty )
        {
            await GroceryListService.UpdateAsync( GroceryList! );
            StateHasChanged();
        }
    }

    private async void OnToggledChanged( bool toggled )
    {
        if ( GroceryList != null && GroceryList.Id != Guid.Empty )
        {
            GroceryList.IsPrinted = toggled;

            await GroceryListService.UpdateAsync( GroceryList! );

            SnackBar.Add( GroceryList.IsPrinted ? $"{GroceryList.Name} marked complete." : $"{GroceryList.Name} marked incomplete.", MudBlazor.Severity.Normal );

            StateHasChanged();
        }
    }

    private async Task AddToStore( Guid id )
    {
        GroceryItem? groceryItem = null;

        var groceryListItem = GroceryList!.Items.FirstOrDefault( i => i.Id == id );
        if ( groceryListItem != null )
        {
            groceryItem = groceryListItem!.GroceryItem;
            if ( groceryItem == null )
            {
                groceryItem = groceryListItem!.RecipeGroceryItem!.GroceryItem;
            }
        }

        var parameters = new DialogParameters<GroceryItemToStoreDialog>
        {
            { x => x.GroceryStore, _groceryStore },
            { x => x.GroceryItem, groceryItem }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var task = await DialogService.ShowAsync<GroceryItemToStoreDialog>( "Dialog", parameters, options );

        var result = await task.Result;
        if ( result != null && !result.Canceled )
        {
            GroceryList = await GroceryListService.GetByIdAsync( Id!.Value );
            GroceryStoreChanged( _groceryStore );
            StateHasChanged();

            SnackBar.Add( $"{groceryItem!.Name} add to {_groceryStore!.Name}" );
        }
    }

    /// <summary>
    /// Removes an item from the grocery list.
    /// </summary>
    /// <param name="id">The ID of the item to remove.</param>
    private async Task RemoveItemAsync( Guid id )
    {
        var itemToRemove = GroceryList!.Items.First( i => i.Id == id );

        GroceryList!.Items.Remove( itemToRemove );
        await GroceryListService.RemoveItemAsync( id );

        GroceryList!.Items = GroceryList!.Items.OrderBy( i => i.SourceRecipe?.Name ?? "zz" ).ToList();

        _recipeDropItems = await GetRecipesDropItemsAsync();

        StateHasChanged();
    }

    private List<RecipeDropItem>? _recipeDropItems = new();

    /// <summary>
    /// Represents a recipe item for the drop-down list.
    /// </summary>
    public class RecipeDropItem
    {
        public Guid Id { get; set; }
        public string Name { get; init; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
    }

    /// <summary>
    /// Fetches the available recipes for the drop-down list.
    /// </summary>
    /// <returns>A list of recipe drop items.</returns>
    public async Task<List<RecipeDropItem>> GetRecipesDropItemsAsync()
    {
        var recipes = await RecipeService.SearchAsync( _searchTerm, 25 );

        // only include recipes with items. TODO: This should be handled by the handler
        recipes = recipes.Where( r => r.RecipeGroceryItems.Any() ).ToList();

        var recipesInList = GroceryList!.Items
            .Where( i => i.SourceRecipe?.Id != null )
            .Select( i => i.SourceRecipe!.Id )
            .ToList();

        return recipes
            .Where( r => !recipesInList.Contains( r.Id ) )
            .Select( r => new RecipeDropItem
            {
                Id = r.Id,
                Name = r.Name,
                ImageUrl = r.Image?.Url ?? string.Empty,
                Identifier = "Recipes"
            }
            ).ToList();
    }

    private async Task<IEnumerable<GroceryStore>> SearchStores( string value, CancellationToken token )
    {
        var _groceryStores = await GroceryStoreService.GetListAsync();
        if ( _groceryStores == null )
        {
            _groceryStores = new List<GroceryStore>();
        }

        if ( string.IsNullOrEmpty( value ) )
        {
            return _groceryStores;
        }

        return _groceryStores.Where( i => i.Name.Contains( value, StringComparison.InvariantCultureIgnoreCase ) );
    }

    private void Print()
    {
        if ( _groceryStore != null )
        {
            NavigationManager.NavigateTo( $"/grocerylist/{GroceryList!.Id}/print/{_groceryStore?.Id}" );
        }
        else
        {
            NavigationManager.NavigateTo( $"/grocerylist/{GroceryList!.Id}/print" );
        }
    }

    public async Task SearchAsync()
    {
        _recipeDropItems = await GetRecipesDropItemsAsync();
        StateHasChanged();
    }

    private async Task<IEnumerable<GroceryItem>> SearchGroceryItems( string value, CancellationToken token )
    {
        _groceryItems = await GroceryItemService.GetListAsync();
        if ( _groceryItems == null )
        {
            _groceryItems = new List<GroceryItem>();
        }

        if ( string.IsNullOrEmpty( value ) )
        {
            return _groceryItems;
        }

        return _groceryItems.Where( i => i.Name.Contains( value, StringComparison.InvariantCultureIgnoreCase ) );
    }

    private void GroceryStoreChanged( GroceryStore? store )
    {
        if ( store != null )
        {
            _groceryItemsAtStore = store.GroceryStoreAisles.SelectMany( a => a.GroceryItems ).ToList();
        }
        else
        {
            _groceryItemsAtStore = new();
        }

        _groceryStore = store;
        StateHasChanged();
    }

    private TableGroupDefinition<GroceryListItem> _groupDefinition = new()
    {
        GroupName = "Group",
        Indentation = false,
        Expandable = false,
        Selector = ( e ) => e.SourceRecipe?.Name ?? "Misc"
    };

    #region Add Grocery Item Dialog

    private ICollection<GroceryItem>? _groceryItems;
    private GroceryItem? _selectedGroceryItem;
    private int _qty = 1;
    private MudAutocomplete<GroceryItem> _autocomplete = new MudAutocomplete<GroceryItem>();
    private bool _addDialogVisible;
    private string _newGroceryItemName { get; set; } = string.Empty;

    /// <summary>
    /// Adds a new item to the grocery list.
    /// </summary>
    private async void AddItemAsync( GroceryItem item )
    {
        if ( GroceryList != null && GroceryList.Id != Guid.Empty && item != null )
        {
            GroceryListItem groceryListItem = new GroceryListItem
            {
                Quantity = _qty,
                GroceryItem = item
            };

            groceryListItem.Id = await GroceryListService.AddItemAsync( GroceryList!.Id, groceryListItem );
            GroceryList!.Items.Add( groceryListItem );

            _selectedGroceryItem = null;
            _newGroceryItemName = string.Empty;

            await _autocomplete.CloseMenuAsync();
            StateHasChanged();
        }
    }

    private void OpenAddDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,

            MaxWidth = MaxWidth.Large
        };

        _addDialogVisible = true;
    }

    private async void Submit()
    {
        var newGroceryItemGuid = await GroceryItemService.CreateAsync( new GroceryItem
        {
            Name = _newGroceryItemName
        } );

        _addDialogVisible = false;

        _groceryItems = await GroceryItemService.GetListAsync();
        AddItemAsync( _groceryItems.FirstOrDefault( i => i.Id == newGroceryItemGuid )! );

        await _autocomplete.CloseMenuAsync();

        StateHasChanged();
    }

    private void Cancel() => _addDialogVisible = false;

    #endregion
}
