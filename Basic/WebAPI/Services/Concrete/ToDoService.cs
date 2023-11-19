using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Extensions;
using WebAPI.Models;
using WebAPI.ModelViews;
using WebAPI.Services.Abstract;

namespace WebAPI.Services.Concrete;

public class ToDoService : ITodoService
{
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ToDoService(
        ApplicationDBContext applicationDBContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _applicationDBContext = applicationDBContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Add(ToDoRequest toDoRequest)
    {
        var userId = GetUserId();
        var user = await _applicationDBContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null) throw new BadHttpRequestException("User does not exist");

        var todo = new ToDo()
        {
            Title = toDoRequest.Title,
            Deadline = toDoRequest.Deadline,
            Description = toDoRequest.Description,
            CreatedAt = DateTime.UtcNow,
            User = user,
        };

        await _applicationDBContext.ToDos.AddAsync(todo);
        await _applicationDBContext.SaveChangesAsync();
    }

    public async Task<List<ToDo>> GetAll()
    {
        var userId = GetUserId();
        var todos = await _applicationDBContext.ToDos
            .Where(todo => todo.UserId == userId)
            .ToListAsync();

        //var result = from todo in _applicationDBContext.ToDos
        //             where todo.UserId == userId
        //             select todo;

        //var todos = await result.ToListAsync();

        return todos;
    }

    public async Task Update(TodoUpdateRequest toDoRequest)
    {
        var userId = GetUserId();
        var todo = await _applicationDBContext.ToDos
            .FirstOrDefaultAsync(todo => todo.Id == toDoRequest.Id);

        if (todo is null)
            throw new BadHttpRequestException("Todo does not exist");

        CheckIfTodoBelongsToUser(todo, userId);

        todo.UpdatedAt = DateTime.UtcNow;
        todo.Description = toDoRequest.Description;
        todo.Title = toDoRequest.Title;
        todo.IsCompleted = toDoRequest.IsCompleted;

        await _applicationDBContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var userId = GetUserId();
        var todo = _applicationDBContext.ToDos
            .FirstOrDefault(todo => todo.Id == id);

        if (todo is null)
            throw new BadHttpRequestException("Todo does not exist");

        CheckIfTodoBelongsToUser(todo, userId);

        _applicationDBContext.ToDos.Remove(todo);
        await _applicationDBContext.SaveChangesAsync();
    }

    private void CheckIfTodoBelongsToUser(ToDo todo, int userId)
    {
        if (todo.UserId != userId)
            throw new BadHttpRequestException("This todo does not belong to this user");
    }

    public async Task<ToDo> GetById(int id)
    {
        var userId = GetUserId();
        var todo = await _applicationDBContext.ToDos.FirstOrDefaultAsync(todo => todo.Id == id);

        if (todo is null)
            throw new BadHttpRequestException("Todo does not exist");

        return todo;
    }

    private int GetUserId() => _httpContextAccessor.HttpContext.User.GetUserId();
}
