using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;

namespace HomeFlow.Features.People.Contacts;

public record CreateContactCommand( Contact Contact ) : IRequest<Guid>;

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public CreateContactCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle( CreateContactCommand request, CancellationToken cancellationToken )
    {
        var entity = new ContactEntity
        {
            FirstName = request.Contact.FirstName,
            LastName = request.Contact.LastName,
            Email = request.Contact.Email,
            PhoneNumber = request.Contact.PhoneNumber,
            BirthDate = request.Contact.BirthDate,
            AnniversaryDate = request.Contact.AnniversaryDate
        };

        if ( request.Contact.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.Contact.Image.Id );
            entity.Image = imageEntity;
        }

        _context.Contacts.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
