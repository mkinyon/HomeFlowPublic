

using HomeFlow.Extensions;
using HomeFlow.Features.MealPlanning.GroceryStores;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class GroceryStoreDetail
{
    [Inject]
    IGroceryStoreService GroceryStoreService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    public GroceryStore? GroceryStore = null;

    GroceryStoreAisle? _selectedGroceryStoreAisle = null;

    private MudDropContainer<GroceryStoreAisle> _dropContainer = new();
    private MudForm _form = new();
    private GroceryStoreValidator validator = new GroceryStoreValidator();

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            GroceryStore = await GroceryStoreService.GetByIdAsync( Id.Value );
        }
        else
        {
            GroceryStore = new GroceryStore();
        }
    }

    private void ReorderItems( MudItemDropInfo<GroceryStoreAisle> dropInfo )
    {
        if ( GroceryStore == null || dropInfo.Item == null )
        {
            return;
        }

        int oldIndex = GroceryStore.GroceryStoreAisles.IndexOf( dropInfo.Item );
        if ( oldIndex < 0 )
            return;

        GroceryStore.GroceryStoreAisles.Reorder( oldIndex, dropInfo.IndexInZone );
    }

    private async Task AddAisle()
    {
        var newAisle = new GroceryStoreAisle { Name = "new aisle" };
        GroceryStore!.GroceryStoreAisles.Add( newAisle );
        _selectedGroceryStoreAisle = newAisle;

        await OpenDialog();
        _dropContainer.Refresh();
    }

    private async Task EditAisle( Guid id )
    {
        _selectedGroceryStoreAisle = GroceryStore!.GroceryStoreAisles.First( i => i.Id == id );

        await OpenDialog();
        _dropContainer.Refresh();
    }

    private void DeleteAisle( Guid id )
    {
        GroceryStore!.GroceryStoreAisles.Remove( GroceryStore!.GroceryStoreAisles.First( i => i.Id == id ) );

        _dropContainer.Refresh();
        StateHasChanged();
    }

    private async Task OpenDialog()
    {
        var parameters = new DialogParameters<GroceryAisleDialog>
        {
            { x => x.Aisle, _selectedGroceryStoreAisle }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var task = await DialogService.ShowAsync<GroceryAisleDialog>( "Dialog", parameters, options );

        // Ensures that the aisle list is repainted when changes are made
        var result = await task.Result;
        if ( result != null && !result.Canceled )
        {
            StateHasChanged();
        }
    }

    private async Task Save()
    {
        await _form.Validate();
        if ( !_form.IsValid )
        {
            return;
        }

        if ( GroceryStore == null )
        {
            return;
        }

        if ( GroceryStore?.Id == Guid.Empty )
        {
            GroceryStore.Id = await GroceryStoreService.CreateAsync( GroceryStore );
        }
        else
        {
            await GroceryStoreService.UpdateAsync( GroceryStore! );
        }

        NavigationManager.NavigateTo( $"grocerystores" );
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo( "grocerystores" );
    }
}