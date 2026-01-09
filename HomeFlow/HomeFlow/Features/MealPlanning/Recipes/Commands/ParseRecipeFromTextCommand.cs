using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Infrastructure.AI;

namespace HomeFlow.Features.MealPlanning.Recipes.Commands;

public record ParseRecipeFromTextCommand( string RecipeText ) : IRequest<Recipe?>;

public class ParseRecipeFromTextCommandHandler : IRequestHandler<ParseRecipeFromTextCommand, Recipe?>
{
    private readonly IGroceryItemService _groceryItemService;
    private readonly IAIService _aiService;
    private readonly IMapper _mapper;

    public ParseRecipeFromTextCommandHandler( IGroceryItemService groceryItemService, IAIService aiService, IMapper mapper )
    {
        _groceryItemService = groceryItemService;
        _aiService = aiService;
        _mapper = mapper;
    }

    public async Task<Recipe?> Handle( ParseRecipeFromTextCommand request, CancellationToken cancellationToken )
    {
        var groceryItems = await _groceryItemService.GetListAsync();
        var groceryItemNames = groceryItems.Select( item => item.Name ).ToList();

        var prompt = $@"
Please take this text and convert into a recipe EXACTLY as it is written. {request.RecipeText}

- Tags should only include the ethnicity of the dish and whether or not it is easy to cook or healthy.
- Also, please properly capitalize the grocery item names if you are not doing a direct match.
- Also, use this list of grocery items to look for existing grocery items. If you find a match, please use the same name from that grocery item: {string.Join( "^", groceryItemNames )}";

        var aiRecipe = await _aiService.GetStructuredResponse<AIRecipe>( prompt );
        var recipe = _mapper.Map<Recipe>( aiRecipe );

        return recipe;
    }
}