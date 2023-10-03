using WebAPI.ModelViews;
using WebAPI.Services.Abstract;

namespace WebAPI.Endpoints;

public static class ToDoEndpoints
{
    public static void RegisterToDoEndpoints(this WebApplication app)
    {
        var todo = app.MapGroup("todos")
            .RequireAuthorization()
            .WithTags("Todos");

        todo.MapPost("",
            async (ITodoService todoService, ToDoRequest toDoRequest) =>
            {
                await todoService.Add(toDoRequest);
                return Results.Ok();
            });

        todo.MapPut("",
            async(ITodoService todoService, TodoUpdateRequest todoUpdateRequest) =>
            {
                await todoService.Update(todoUpdateRequest);
                return Results.Ok();
            });

        todo.MapGet("", 
            async (ITodoService todoService) =>
            {
                var result = await todoService.GetAll();
                return Results.Ok(result);
            });
    }
}
