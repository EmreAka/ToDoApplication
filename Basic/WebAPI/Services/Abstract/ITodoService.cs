using WebAPI.Models;
using WebAPI.ModelViews;

namespace WebAPI.Services.Abstract;

public interface ITodoService
{
    Task Add(ToDoRequest toDoRequest);
    Task Update(TodoUpdateRequest toDoRequest);
    Task<List<ToDo>> GetAll();
}
