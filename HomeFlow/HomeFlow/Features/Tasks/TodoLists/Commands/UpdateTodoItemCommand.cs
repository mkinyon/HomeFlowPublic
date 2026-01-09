using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record UpdateTodoItemCommand( TodoItem TodoItem ) : IRequest;

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateTodoItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateTodoItemCommand request, CancellationToken cancellationToken )
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync( ti => ti.Id == request.TodoItem.Id, cancellationToken );

        if ( todoItem == null )
        {
            throw new ArgumentException( $"TodoItem with ID {request.TodoItem.Id} not found." );
        }

        todoItem.Title = request.TodoItem.Title;
        
        // Handle completion status and datetime
        var wasCompleted = todoItem.IsCompleted;
        todoItem.IsCompleted = request.TodoItem.IsCompleted;
        
        if (request.TodoItem.IsCompleted && !wasCompleted)
        {
            // Item is being marked as completed for the first time
            todoItem.CompletedDateTime = DateTime.UtcNow;
        }
        else if (!request.TodoItem.IsCompleted)
        {
            // Item is being marked as incomplete
            todoItem.CompletedDateTime = null;
        }
        
        todoItem.DueDate = request.TodoItem.DueDate;
        todoItem.Order = request.TodoItem.Order;

        // Handle AssignedFamilyMemberId with validation
        if ( request.TodoItem.AssignedFamilyMember?.Id != null )
        {
            // Verify the family member exists in the database
            var familyMemberExists = await _context.FamilyMembers
                .AnyAsync( fm => fm.Id == request.TodoItem.AssignedFamilyMember.Id, cancellationToken );
            
            if ( !familyMemberExists )
            {
                throw new ArgumentException( $"Family member with ID {request.TodoItem.AssignedFamilyMember.Id} not found." );
            }
            
            todoItem.AssignedFamilyMemberId = request.TodoItem.AssignedFamilyMember.Id;
        }
        else
        {
            // Clear the assignment if no family member is provided
            todoItem.AssignedFamilyMemberId = null;
        }
        
        await _context.SaveChangesAsync( cancellationToken );
    }
}
