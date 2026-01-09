using HomeFlow.Services;

namespace HomeFlow.Features.People.Contacts;

public interface IContactService : IService<Contact>
{
    public Task<List<UpcomingMilestoneVM>> GetUpcomingMilestones();
}

public class ContactService : BaseService<Contact>, IContactService
{
    public ContactService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( Contact item ) =>
        _mediator.Send( new CreateContactCommand( item ) );

    public override Task UpdateAsync( Contact item ) =>
        _mediator.Send( new UpdateContactCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteContactCommand( id ) );

    public override Task<Contact?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetContactById( id ) );

    public override Task<List<Contact>> GetListAsync() =>
        _mediator.Send( new GetContactsQuery() );

    public override Task<TableData<Contact>> GetTableDataAsync( QueryOptions options ) =>
        _mediator.Send( new GetContactTableDataQuery( options ) );

    public Task<List<UpcomingMilestoneVM>> GetUpcomingMilestones() =>
        _mediator.Send( new GetUpcomingMilestonesQuery() );
}
