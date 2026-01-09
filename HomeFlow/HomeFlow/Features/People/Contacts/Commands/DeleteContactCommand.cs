using HomeFlow.Data;

namespace HomeFlow.Features.People.Contacts;

public record DeleteContactCommand( Guid Id ) : IRequest;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteContactCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteContactCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.Contacts
            .FirstOrDefaultAsync( x => x.Id == request.Id, cancellationToken );

        if ( entity == null )
        {
            throw new ArgumentException( $"Contact with ID {request.Id} not found." );
        }

        _context.Contacts.Remove( entity );

        await _context.SaveChangesAsync( cancellationToken );
    }
}
