using WebAPI.Models;
using WebAPI.ModelViews;

namespace WebAPI.Services.Abstract;

public interface ITodoService
{
    Task Add(ToDoRequest toDoRequest);
    Task Update(TodoUpdateRequest toDoRequest);
    Task Delete(int id);
    Task<List<ToDo>> GetAll();
    Task<ToDo> GetById(int id);
}
