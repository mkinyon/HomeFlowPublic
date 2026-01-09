using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;

namespace HomeFlow.Features.People.Contacts;

public record UpdateContactCommand( Contact Contact ) : IRequest;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UpdateContactCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task Handle( UpdateContactCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.Contacts
            .Include( x => x.Image )
            .FirstOrDefaultAsync( x => x.Id == request.Contact.Id, cancellationToken );

        if ( entity == null )
        {
            throw new ArgumentException( $"Contact with ID {request.Contact.Id} not found." );
        }

        entity.FirstName = request.Contact.FirstName;
        entity.LastName = request.Contact.LastName;
        entity.Email = request.Contact.Email;
        entity.PhoneNumber = request.Contact.PhoneNumber;
        entity.BirthDate = request.Contact.BirthDate;
        entity.AnniversaryDate = request.Contact.AnniversaryDate;

        if ( request.Contact.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.Contact.Image.Id );
            entity.Image = imageEntity;
        }
        else
        {
            entity.Image = null;
        }

        await _context.SaveChangesAsync( cancellationToken );
    }
}
