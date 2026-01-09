using HomeFlow.Features.People.Contacts;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Widgets
{
    public partial class UpcomingMilestonesWidget
    {
        [Inject]
        public IContactService ContactService { get; set; } = default!;

        public List<UpcomingMilestoneVM> Milestones { get; set; } = new();
        public List<UpcomingMilestoneVM> CurrentDayMilestones { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var today = DateTime.Now.Date;
            Milestones = await ContactService.GetUpcomingMilestones();

            // Filter for current day milestones
            CurrentDayMilestones = Milestones.Where( m => m.IsToday ).ToList();
        }
    }
}
