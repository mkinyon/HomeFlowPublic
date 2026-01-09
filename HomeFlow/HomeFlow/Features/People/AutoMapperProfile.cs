
namespace HomeFlow.Features.People;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<Features.People.Contacts.Contact, Features.People.Contacts.ContactEntity>().ReverseMap();
    }
}
