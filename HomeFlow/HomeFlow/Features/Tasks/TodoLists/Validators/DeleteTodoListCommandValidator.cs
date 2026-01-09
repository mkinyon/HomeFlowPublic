namespace HomeFlow.Features.Tasks.TodoLists;

public class DeleteTodoListCommandValidator : AbstractValidator<DeleteTodoListCommand>
{
    public DeleteTodoListCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo list ID is required.");
    }
}
