using HomeFlow.Features.Tasks.TodoLists;

namespace HomeFlow.Features.Tasks;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<TodoList, TodoListEntity>().ReverseMap();
        CreateMap<TodoItem, TodoItemEntity>().ReverseMap();
    }
}
