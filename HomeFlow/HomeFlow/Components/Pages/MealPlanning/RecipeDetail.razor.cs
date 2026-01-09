
using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Features.Core.Tags;
using HomeFlow.Features.MealPlanning.Recipes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class RecipeDetail
{
    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Inject]
    ITagService TagService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    private bool _editMode = false;

    public string RecipeName { get; set; } = "New Recipe";

    public Recipe? Recipe = null;

    private ImageFile? _image { get; set; }

    MudForm form = new MudForm();
    private RecipeValidator validator = new RecipeValidator();
    private MudMessageBox _mudMessageBox = new MudMessageBox();

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            Recipe = await RecipeService.GetByIdAsync( Id.Value );
            RecipeName = Recipe?.Name ?? "";
            _image = Recipe?.Image;
        }
        else
        {
            Recipe = new Recipe { Name = "New Recipe", Description = "", RecipeType = RecipeType.MainDish };
            RecipeName = Recipe.Name;
            _editMode = true;
        }
    }

    private async Task SaveRecipe()
    {
        await form.Validate();
        if ( !form.IsValid )
        {
            return;
        }

        if ( Recipe == null )
        {
            return;
        }

        if ( Recipe?.Id == Guid.Empty )
        {
            Recipe.Id = await RecipeService.CreateAsync( Recipe );
            NavigationManager.NavigateTo( $"recipe/{Recipe?.Id}" );
        }
        else
        {
            await RecipeService.UpdateAsync( Recipe! );
        }

        await TagService.UpdateAsync( "Recipe", Recipe!.Id, Recipe.Tags );

        _editMode = false;
    }

    private void Cancel()
    {
        if ( Recipe?.Id != null )
        {
            _editMode = false;
            NavigationManager.NavigateTo( $"recipe/{Recipe?.Id}" );
        }
        else
        {
            NavigationManager.NavigateTo( "recipes" );
        }
    }

    private void TitleChange()
    {
        RecipeName = Recipe?.Name ?? string.Empty;
    }

    private void OnTagsChanged( List<string> updatedTags )
    {
        if ( Recipe != null )
        {
            Recipe.Tags = updatedTags;
        }
    }

    private async Task OpenAIRecipeImport()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<AIRecipeImportDialog>( "AI Recipe Import", options );
        var result = await dialog.Result;

        if ( !result.Canceled && result.Data is Recipe importedRecipe )
        {
            // Replace the current recipe with the imported data (no saving)
            Recipe = importedRecipe;
            RecipeName = Recipe?.Name ?? string.Empty;
            _image = Recipe?.Image;

            // Stay in edit mode for further editing
            StateHasChanged();
        }
    }

    private async Task Print()
    {
        if ( Recipe?.Id != null )
        {
            _editMode = false;
            NavigationManager.NavigateTo( $"recipe/{Recipe?.Id}/print" );
        }
    }

    private async Task DeleteRecipe()
    {
        if ( Recipe?.Id == null || Recipe.Id == Guid.Empty )
        {
            return;
        }

        bool? result = await _mudMessageBox.ShowAsync();
        string state = result is null ? "Canceled" : "Deleted!";

        if ( state == "Deleted!" )
        {
            await RecipeService.DeleteAsync( Recipe.Id );
            NavigationManager.NavigateTo( "recipes" );
        }
    }
}