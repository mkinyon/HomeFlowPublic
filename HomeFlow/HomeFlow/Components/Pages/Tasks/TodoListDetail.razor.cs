using HomeFlow.Features.Core.FamilyMembers;
using HomeFlow.Features.Tasks.TodoLists;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace HomeFlow.Components.Pages.Tasks;

public partial class TodoListDetail
{
    [Inject]
    ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    ITodoListsService TodoListsService { get; set; } = default!;

    [Inject]
    IFamilyMemberService FamilyMemberService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public Guid? Id { get; set; }

    public TodoList? TodoList { get; set; }
    private List<FamilyMember> _familyMembers = new();
    private string _newItemTitle = string.Empty;

    /// <summary>
    /// Sorts todo items: incomplete first (by order), then completed (by order)
    /// </summary>
    private void SortTodoItems()
    {
        if (TodoList?.Items != null)
        {
            TodoList.Items = TodoList.Items
                .OrderBy(i => i.IsCompleted)
                .ThenBy(i => i.Order)
                .ToList();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // Load family members for assignment dropdown
        _familyMembers = await FamilyMemberService.GetListAsync();

        if ( Id.HasValue && Id.Value != Guid.Empty )
        {
            TodoList = await TodoListsService.GetByIdAsync( Id.Value );
            if ( TodoList != null )
            {
                SortTodoItems();
            }
        }
        else
        {
            // Create new todo list
            TodoList = new TodoList
            {
                Name = $"Todo List {DateTime.Now:MMM dd, yyyy}"
            };

            var newId = await TodoListsService.CreateAsync( TodoList );
            TodoList.Id = newId;
            NavigationManager.NavigateTo( $"/todolist/{newId}", replace: true );
        }
    }

    private async void UpdateTodoListName()
    {
        if ( TodoList != null )
        {
            if ( TodoList.Id == Guid.Empty )
            {
                // Create new todo list
                var newId = await TodoListsService.CreateAsync( TodoList );
                TodoList.Id = newId;
                NavigationManager.NavigateTo( $"/todolist/{newId}", replace: true );
            }
            else
            {
                await TodoListsService.UpdateAsync( TodoList );
            }
            SnackBar.Add( "Todo list updated.", Severity.Success );
        }
    }

    private async void AddNewItem()
    {
        if ( TodoList != null && !string.IsNullOrWhiteSpace( _newItemTitle ) )
        {
            var newItem = new TodoItem
            {
                Title = _newItemTitle,
                Order = TodoList.Items.Count
            };

            if ( TodoList.Id != Guid.Empty )
            {
                var newItemId = await TodoListsService.AddTodoItemAsync( TodoList.Id, newItem );
                newItem.Id = newItemId;
                TodoList.Items.Add( newItem );
            }
            else
            {
                TodoList.Items.Add( newItem );
            }

            SortTodoItems();

            _newItemTitle = string.Empty;
            StateHasChanged();
        }
    }

    private async void EditItem( TodoItem item )
    {
        var parameters = new DialogParameters
        {
            { "TodoItem", item },
            { "FamilyMembers", _familyMembers }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<TodoItemEditDialog>( "Edit Todo Item", parameters, options );
        var result = await dialog.Result;

        if ( !result.Canceled && TodoList != null && TodoList.Id != Guid.Empty )
        {
            await TodoListsService.UpdateTodoItemAsync( item );
            SnackBar.Add( "Todo item updated.", Severity.Success );
            StateHasChanged();
        }
    }

    private async void RemoveItemAsync( Guid itemId )
    {
        if ( TodoList != null && TodoList.Id != Guid.Empty )
        {
            await TodoListsService.RemoveTodoItemAsync( TodoList.Id, itemId );
            TodoList.Items.RemoveAll( i => i.Id == itemId );
            SnackBar.Add( "Item removed.", Severity.Success );
            StateHasChanged();
        }
        else
        {
            TodoList?.Items.RemoveAll( i => i.Id == itemId );
            StateHasChanged();
        }
    }

    private async void ToggleItemStatus( TodoItem item, bool value )
    {
        item.IsCompleted = value;

        SortTodoItems();

        if ( TodoList != null && TodoList.Id != Guid.Empty )
        {
            await TodoListsService.UpdateTodoItemAsync( item );
            SnackBar.Add( $"Item {(item.IsCompleted ? "completed" : "marked incomplete")}.", Severity.Success );
        }

        StateHasChanged();
    }

    private async void OnItemTitleChanged( TodoItem item, string newTitle )
    {
        // Only update if the title actually changed and we have a valid todo list
        if ( item.Title != newTitle && TodoList != null && TodoList.Id != Guid.Empty )
        {
            item.Title = newTitle;
            await TodoListsService.UpdateTodoItemAsync( item );
            SnackBar.Add( "Item title updated.", Severity.Success );
        }
        else if ( item.Title != newTitle )
        {
            // Update the local value even if we don't save to database
            item.Title = newTitle;
        }
    }

    private async void CopyTodoList()
    {
        if ( TodoList == null || TodoList.Id == Guid.Empty )
            return;

        try
        {
            // Use the dedicated copy command
            Id = await TodoListsService.CopyTodoListAsync( TodoList.Id );

            // Navigate to the new todo list
            NavigationManager.NavigateTo( $"/todolist/{Id}", replace: true );

            TodoList = await TodoListsService.GetByIdAsync( Id.Value );

            SnackBar.Add( "Todo list copied successfully!", Severity.Success );
        }
        catch ( Exception ex )
        {
            SnackBar.Add( $"Error copying todo list: {ex.Message}", Severity.Error );
        }
    }
}
