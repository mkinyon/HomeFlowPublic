using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public record DeleteMealPlannerItemCommand( Guid Id ) : IRequest;

public class DeleteMealPlannerItemCommandHandler : IRequestHandler<DeleteMealPlannerItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteMealPlannerItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteMealPlannerItemCommand request, CancellationToken cancellationToken )
    {
        var mealPlannerItemEntity = await _context.MealPlannerItems.FirstOrDefaultAsync( gl => gl.Id == request.Id );

        Guard.Against.NotFound( request.Id, mealPlannerItemEntity );

        _context.MealPlannerItems.Remove( mealPlannerItemEntity );

        await _context.SaveChangesAsync( cancellationToken );
    }
}
