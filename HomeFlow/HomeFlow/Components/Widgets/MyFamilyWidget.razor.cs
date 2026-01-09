
using HomeFlow.Features.Core.FamilyMembers;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Widgets;

public partial class MyFamilyWidget
{
    [Inject]
    public IFamilyMemberService FamilyMemberService { get; set; } = default!;

    public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();

    protected override async Task OnInitializedAsync()
    {
        FamilyMembers = await FamilyMemberService.GetListAsync();
    }
}
