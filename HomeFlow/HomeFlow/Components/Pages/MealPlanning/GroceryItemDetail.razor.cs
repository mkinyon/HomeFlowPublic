
using HomeFlow.Features.Core.Tags;
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.GroceryLists;
using HomeFlow.Features.MealPlanning.Recipes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryItemDetail
{
    [Inject]
    IGroceryItemService GroceryItemService { get; set; } = default!;

    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Inject]
    IGroceryListService GroceryListService { get; set; } = default!;

    [Inject]
    ITagService TagService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    public GroceryItem? GroceryItem = null;

    private MudForm _form = new();
    private GroceryItemValidator validator = new GroceryItemValidator();

    private List<Recipe> _recipes = new();
    private List<GroceryList> _groceryLists = new();

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            GroceryItem = await GroceryItemService.GetByIdAsync( Id.Value );
            _recipes = await RecipeService.GetByGroceryItemQuery( GroceryItem!.Id );
            _groceryLists = await GroceryListService.GetByGroceryItemQuery( GroceryItem!.Id );
        }
        else
        {
            GroceryItem = new GroceryItem();
        }
    }

    private void OnTagsChanged( List<string> updatedTags )
    {
        GroceryItem!.Tags = updatedTags;
    }

    private async Task Save()
    {
        await _form.Validate();
        if ( !_form.IsValid )
        {
            return;
        }

        if ( GroceryItem == null )
        {
            return;
        }

        if ( GroceryItem?.Id == Guid.Empty )
        {
            GroceryItem.Id = await GroceryItemService.CreateAsync( GroceryItem );
        }
        else
        {
            await GroceryItemService.UpdateAsync( GroceryItem! );
        }

        await TagService.UpdateAsync( "GroceryItem", GroceryItem!.Id, GroceryItem.Tags );

        NavigationManager.NavigateTo( $"groceryitems" );
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo( "groceryitems" );
    }

    bool _expanded = false;

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }

    private void NavigateToRecipe( TableRowClickEventArgs<Recipe> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"recipe/{args.Item.Id}" );
        }
    }

    private void NavigateToGroceryList( TableRowClickEventArgs<GroceryList> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"grocerylist/{args.Item.Id}" );
        }
    }
}
