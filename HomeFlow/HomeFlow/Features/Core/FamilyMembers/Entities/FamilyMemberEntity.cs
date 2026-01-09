using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Models;

namespace HomeFlow.Features.Core.FamilyMembers
{
    public class FamilyMemberEntity : Model
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public Guid? ImageId { get; set; }

        public Gender Gender { get; set; }

        public FamilyMemberType FamilyMemberType { get; set; }

        public virtual ImageFileEntity? Image { get; set; }
    }
}
