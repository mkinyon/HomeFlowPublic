namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoListValidator : AbstractValidator<TodoList>
{
    public TodoListValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The todo list name is required.")
            .MaximumLength(100).WithMessage("The todo list name cannot exceed 100 characters.");

        RuleForEach(x => x.Items)
            .SetValidator(new TodoItemValidator());
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<TodoList>.CreateWithOptions((TodoList)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
