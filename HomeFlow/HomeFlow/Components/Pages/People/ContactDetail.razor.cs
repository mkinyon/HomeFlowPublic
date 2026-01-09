using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Features.People.Contacts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.People;

public partial class ContactDetail
{
    [Inject]
    IContactService ContactService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    private bool _editMode = false;

    public string ContactName { get; set; } = "New Contact";

    public Contact? Contact = null;

    private ImageFile? _image { get; set; }

    MudForm form = new MudForm();
    private ContactValidator validator = new ContactValidator();

    // Date properties for MudDatePicker binding
    public DateTime? BirthDate
    {
        get => Contact?.BirthDate?.ToDateTime( TimeOnly.MinValue );
        set => Contact!.BirthDate = value.HasValue ? DateOnly.FromDateTime( value.Value ) : null;
    }

    public DateTime? AnniversaryDate
    {
        get => Contact?.AnniversaryDate?.ToDateTime( TimeOnly.MinValue );
        set => Contact!.AnniversaryDate = value.HasValue ? DateOnly.FromDateTime( value.Value ) : null;
    }

    protected override async Task OnInitializedAsync()
    {
        if ( Id.HasValue )
        {
            Contact = await ContactService.GetByIdAsync( Id.Value );
            ContactName = Contact?.FullName ?? "";
            _image = Contact?.Image;
        }
        else
        {
            Contact = new Contact { FirstName = "", LastName = "" };
            ContactName = "New Contact";
            _editMode = true;
        }
    }

    private async Task SaveContact()
    {
        await form.Validate();
        if ( !form.IsValid )
        {
            return;
        }

        if ( Contact == null )
        {
            return;
        }

        if ( Contact?.Id == Guid.Empty )
        {
            Contact.Id = await ContactService.CreateAsync( Contact );
            NavigationManager.NavigateTo( $"contact/{Contact?.Id}" );
        }
        else
        {
            await ContactService.UpdateAsync( Contact! );
        }

        _editMode = false;
    }

    private void Cancel()
    {
        if ( Contact?.Id != null && Contact.Id != Guid.Empty )
        {
            _editMode = false;
            NavigationManager.NavigateTo( $"contact/{Contact?.Id}" );
        }
        else
        {
            NavigationManager.NavigateTo( "contacts" );
        }
    }

    private void NameChange()
    {
        ContactName = Contact?.FullName ?? string.Empty;
    }

    private async Task DeleteContact()
    {
        if ( Contact?.Id == null || Contact.Id == Guid.Empty )
        {
            return;
        }

        var result = await DialogService.ShowMessageBox(
            "Delete Contact",
            $"Are you sure you want to delete {Contact.FullName}?",
            yesText: "Delete",
            noText: "Cancel",
            options: new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            }
        );

        if ( result == true )
        {
            await ContactService.DeleteAsync( Contact.Id );
            NavigationManager.NavigateTo( "contacts" );
        }
    }
}
