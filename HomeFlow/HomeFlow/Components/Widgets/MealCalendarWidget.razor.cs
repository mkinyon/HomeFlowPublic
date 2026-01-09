using HomeFlow.Features.MealPlanning.MealPlannerItems;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Widgets;

public partial class MealCalendarWidget
{
    [Inject]
    public IMealPlannerItemService MealPlannerItemService { get; set; } = default!;

    public List<MealPlannerCalendarDayVM> MealPlannerItems { get; set; } = new();

    public MealPlannerCalendarDayVM? TodaysMeal
        => MealPlannerItems.FirstOrDefault( m => m.Date == DateOnly.FromDateTime( DateTime.Now.Date ) );

    protected override async Task OnInitializedAsync()
    {
        var today = DateTime.Now.Date;
        MealPlannerItems = await MealPlannerItemService.GetByDateRange( today, today.AddDays( 6 ) );
    }
}
