
using HomeFlow.Features.Core.ImageFiles;

namespace HomeFlow.Features.Core.FamilyMembers;

public class FamilyMember
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public ImageFile? Image { get; set; }

    public FamilyMemberType FamilyMemberType { get; set; } = FamilyMemberType.Adult;

    public Gender Gender { get; set; } = Gender.Unset;

    public bool IsValid()
    {
        return FirstName != string.Empty &&
            LastName != string.Empty;
    }

    public string FullName => $"{FirstName} {LastName}";

    public string Initials => $"{FirstName[0]}{LastName[0]}";

    public override string ToString()
    {
        return FullName;
    }
}
