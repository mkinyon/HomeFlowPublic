using HomeFlow.Features.Core.ImageFiles;

namespace HomeFlow.Features.People.Contacts;

public class Contact
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public ImageFile? Image { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    public DateOnly? AnniversaryDate { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Initials => $"{FirstName[0]}{LastName[0]}";

    public int? Age
    {
        get
        {
            if ( BirthDate.HasValue )
            {
                var today = DateOnly.FromDateTime( DateTime.Now );
                var age = today.Year - BirthDate.Value.Year;
                if ( today < BirthDate.Value.AddYears( age ) )
                {
                    age--;
                }
                return age;
            }
            return null;
        }
    }

    public int? AnniversaryYears
    {
        get
        {
            if ( AnniversaryDate.HasValue )
            {
                var today = DateOnly.FromDateTime( DateTime.Now );
                var years = today.Year - AnniversaryDate.Value.Year;
                if ( today < AnniversaryDate.Value.AddYears( years ) )
                {
                    years--;
                }
                return years;
            }
            return null;
        }
    }
}
