using HomeFlow.Features.MealPlanning.Recipes;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Pages.MealPlanning;

public partial class PrintableRecipe
{
    [Inject]
    IRecipeService RecipeService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }


    private Recipe? _recipe = new();
    private int _count = 1;

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            _recipe = await RecipeService.GetByIdAsync( Id.Value );
        }
    }
}
