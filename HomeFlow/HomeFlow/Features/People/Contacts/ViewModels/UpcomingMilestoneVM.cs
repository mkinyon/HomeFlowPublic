namespace HomeFlow.Features.People.Contacts;

public class UpcomingMilestoneVM
{
    public Guid PrimaryContactId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string ShortDate => Date.ToString( "MMM dd" );
    public bool IsBirthday { get; set; }
    public bool IsAnniversary { get; set; }
    public int? Age { get; set; }
    public int? AnniversaryYears { get; set; }
    public bool IsToday { get; set; }
    public SpouseVM? Spouse { get; set; } = null;
}

public class SpouseVM
{
    public Guid ContactId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
}
