
using HomeFlow.Features.MealPlanning.Recipes.Commands;
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.Recipes;

public interface IRecipeService : IService<Recipe>
{
    Task<List<Recipe>> GetRecipeRecommendationsQuery( int count );
    Task<List<Recipe>> SearchAsync( string searchTerm, int count );
    Task<Recipe?> ParseRecipeFromText( string text );
    Task<List<Recipe>> GetByGroceryItemQuery( Guid groceryItemId );
}

public class RecipeService : BaseService<Recipe>, IRecipeService
{
    public RecipeService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( Recipe item ) =>
        _mediator.Send( new CreateRecipeCommand( item ) );

    public override Task UpdateAsync( Recipe item ) =>
        _mediator.Send( new UpdateRecipeCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteRecipeCommand( id ) );

    public override Task<Recipe?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetRecipeById( id ) );

    public override Task<List<Recipe>> GetListAsync() =>
        _mediator.Send( new GetRecipesQuery() );

    public override Task<TableData<Recipe>> GetTableDataAsync( QueryOptions options ) =>
        _mediator.Send( new GetRecipeTableDataQuery( options ) );

    public Task<List<Recipe>> GetRecipeRecommendationsQuery( int count ) =>
        _mediator.Send( new GetRecipeRecommendationsQuery( count ) );

    public Task<List<Recipe>> SearchAsync( string searchTerm, int count ) =>
        _mediator.Send( new SearchRecipesQuery( searchTerm, count ) );

    public Task<Recipe?> ParseRecipeFromText( string text ) =>
        _mediator.Send( new ParseRecipeFromTextCommand( text ) );

    public Task<List<Recipe>> GetByGroceryItemQuery( Guid groceryItemId ) =>
        _mediator.Send( new GetByGroceryItemQuery( groceryItemId ) );
}
