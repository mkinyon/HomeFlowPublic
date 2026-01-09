using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Models;

namespace HomeFlow.Features.People.Contacts;

public class ContactEntity : Model
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    public DateOnly? AnniversaryDate { get; set; }

    public Guid? ImageId { get; set; }

    public virtual ImageFileEntity? Image { get; set; }
}
