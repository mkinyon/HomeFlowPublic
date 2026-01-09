using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record DeleteRecipeCommand( Guid Id ) : IRequest;

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteRecipeCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteRecipeCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.Recipes.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        _context.Recipes.Remove( entity );
        await _context.SaveChangesAsync( cancellationToken );
    }
}
