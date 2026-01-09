

using HomeFlow.Features.Core.FamilyMembers;
using HomeFlow.Features.Core.SystemSettings;
using Microsoft.AspNetCore.Components;

namespace HomeFlow.Components.Pages.Core;

public partial class Setup
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISystemSettingService SystemSettingService { get; set; } = default!;

    [Inject]
    public IFamilyMemberService FamilyMemberService { get; set; } = default!;

    public bool IsValidFamily => _familyMembers.All( fm => fm.IsValid() );

    private List<FamilyMember> _familyMembers = new List<FamilyMember>();

    protected override void OnInitialized()
    {
        FamilyMemberService.GetListAsync().ContinueWith( task =>
        {
            if ( task.Result.Count == 0 )
            {
                _familyMembers.Add( new FamilyMember { } );
            }
            else
            {
                _familyMembers = task.Result.ToList();
            }
            StateHasChanged();
        } );
    }

    private async Task OnFinishAsync()
    {
        var existingFamilyMembers = await FamilyMemberService.GetListAsync();

        foreach ( var familyMember in _familyMembers )
        {
            if ( !familyMember.IsValid() )
            {
                continue;
            }

            if ( !existingFamilyMembers.Any( ef => ef.Id == familyMember.Id ) )
            {
                await FamilyMemberService.CreateAsync( familyMember );
            }
            else if ( existingFamilyMembers.Any( ef => ef.Id == familyMember.Id ) )
            {
                await FamilyMemberService.UpdateAsync( familyMember );
            }
        }

        var oldFamilyMembers = existingFamilyMembers.Where( ef => !_familyMembers.Any( fm => fm.Id == ef.Id ) ).ToList();
        foreach ( var oldFM in oldFamilyMembers )
        {
            await FamilyMemberService.UpdateAsync( oldFM );
        }

        await SystemSettingService.UpdateSystemSettingAsync( new SystemSetting { Key = "IsSetupComplete", Value = "true" } );

        NavigationManager.NavigateTo( "/", true );
    }
}