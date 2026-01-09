using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public record GetMealPlannerItemsByDateRange( DateOnly StartDate, DateOnly EndDate ) : IRequest<List<MealPlannerCalendarDayVM>>;

public class GetMealPlannerItemsQueryHanderer : IRequestHandler<GetMealPlannerItemsByDateRange, List<MealPlannerCalendarDayVM>>
{
    private readonly IHomeFlowDbContext _context;

    public GetMealPlannerItemsQueryHanderer( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
    }

    public async Task<List<MealPlannerCalendarDayVM>> Handle( GetMealPlannerItemsByDateRange request, CancellationToken cancellationToken )
    {
        var mealPlanItems = await _context.MealPlannerItems
            .Include( r => r.Recipe )
            .ThenInclude( r => r.Image )
            .Where( r => r.Date >= request.StartDate && r.Date <= request.EndDate )
            .OrderBy( r => r.Date )
            .ToListAsync();

        var mealPlannerCalendarDays = mealPlanItems
            .Select( r => new MealPlannerCalendarDayVM
            {
                MealPlannerItemId = r.Id,
                Date = r.Date,
                RecipeType = r.Recipe.RecipeType,
                RecipeName = r.Recipe.Name,
                ImageUrl = r.Recipe.Image?.Url ?? string.Empty,
                RecipeId = r.Recipe.Id
            } )
            .ToList();

        return mealPlannerCalendarDays;
    }
}
