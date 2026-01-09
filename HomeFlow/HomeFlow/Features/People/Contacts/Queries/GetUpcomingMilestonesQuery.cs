using HomeFlow.Data;

namespace HomeFlow.Features.People.Contacts;

public record GetUpcomingMilestonesQuery : IRequest<List<UpcomingMilestoneVM>>;

public class GetUpcomingMilestonesQueryHandler : IRequestHandler<GetUpcomingMilestonesQuery, List<UpcomingMilestoneVM>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetUpcomingMilestonesQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UpcomingMilestoneVM>> Handle( GetUpcomingMilestonesQuery request, CancellationToken cancellationToken )
    {
        var contacts = await _context.Contacts
            .Where( c => c.BirthDate.HasValue || c.AnniversaryDate.HasValue )
            .ProjectTo<Contact>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken );

        var bdays = contacts
            .Where( c => c.BirthDate.HasValue )
            .Select( c => new UpcomingMilestoneVM
            {
                PrimaryContactId = c.Id,
                FullName = c.FullName,
                Initials = c.Initials,
                ImageUrl = c.Image?.Url,
                Date = (c.BirthDate!.Value.Month == 2 && c.BirthDate.Value.Day == 29)
                    ? new DateOnly( 1900, 2, 28 )
                    : new DateOnly( 1900, c.BirthDate.Value.Month, c.BirthDate.Value.Day ),
                Age = c.Age,
                IsBirthday = true,
                IsToday = c.BirthDate.Value.Month == DateTime.Now.Month && c.BirthDate.Value.Day == DateTime.Now.Day,
            } )
            .ToList();

        // Group anniversaries by lastname and anniversary date
        var anniversaryGroups = contacts
            .Where( c => c.AnniversaryDate.HasValue )
            .GroupBy( c => new { c.LastName, AnniversaryDate = c.AnniversaryDate!.Value } )
            .ToList();

        var anniversaries = new List<UpcomingMilestoneVM>();

        foreach ( var group in anniversaryGroups )
        {
            var contactsInGroup = group.ToList();
            var primaryContact = contactsInGroup.First();
            var anniversaryDate = group.Key.AnniversaryDate;

            var milestone = new UpcomingMilestoneVM
            {
                PrimaryContactId = primaryContact.Id,
                FirstName = primaryContact.FirstName,
                LastName = primaryContact.LastName,
                FullName = primaryContact.FullName,
                Initials = primaryContact.Initials,
                ImageUrl = primaryContact.Image?.Url,
                Date = (anniversaryDate.Month == 2 && anniversaryDate.Day == 29)
                    ? new DateOnly( 1900, 2, 28 )
                    : new DateOnly( 1900, anniversaryDate.Month, anniversaryDate.Day ),
                AnniversaryYears = primaryContact.AnniversaryYears,
                IsAnniversary = true,
                IsToday = anniversaryDate.Month == DateTime.Now.Month && anniversaryDate.Day == DateTime.Now.Day,
            };

            // If there's a second contact with the same lastname and anniversary date, add them as spouse
            if ( contactsInGroup.Count > 1 )
            {
                var spouseContact = contactsInGroup[1];
                milestone.Spouse = new SpouseVM
                {
                    ContactId = spouseContact.Id,
                    FirstName = spouseContact.FirstName,
                    LastName = spouseContact.LastName,
                    FullName = spouseContact.FullName,
                    Initials = spouseContact.Initials,
                    ImageUrl = spouseContact.Image?.Url
                };

                milestone.FullName = $"{primaryContact.FirstName} & {spouseContact.FirstName} {primaryContact.LastName}";
            }

            anniversaries.Add( milestone );
        }

        var today = new DateOnly( 1900, DateTime.Now.Month, DateTime.Now.Day );

        var upcomingMilestones = bdays
            .Concat( anniversaries )
            .Where( m => m.Date >= today )
            .OrderBy( m => m.Date )
            .Take( 10 )
            .ToList();

        return upcomingMilestones;
    }
}
