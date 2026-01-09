using HomeFlow.Features.People.Contacts;
using HomeFlow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeFlow.Components.Pages.People;

public partial class ContactList
{
    [Inject]
    IContactService ContactService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    private MudTable<Contact> table = default!;
    private string _searchString = "";

    private void ViewContact( TableRowClickEventArgs<Contact> args )
    {
        if ( args.Item != null )
        {
            NavigationManager.NavigateTo( $"/contact/{args.Item.Id}" );
        }
    }

    private async Task<MudBlazor.TableData<Contact>> ContactData( TableState state, CancellationToken cancellationToken )
    {
        var queryOptions = new QueryOptions
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchTerm = _searchString,
            SortBy = state.SortLabel,
            SortDescending = state.SortDirection == SortDirection.Descending
        };

        var response = await ContactService.GetTableDataAsync( queryOptions );

        return new MudBlazor.TableData<Contact>
        {
            Items = response.Items,
            TotalItems = response.TotalItems
        };
    }

    private void SearchAsync()
    {
        table.ReloadServerData();
    }
}
