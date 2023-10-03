using WebAPI.Filters;
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
            }).AddEndpointFilter<ValidatorFilter<ToDoRequest>>();

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

        todo.MapGet("{id:int}", 
            async (ITodoService todoService, int id) =>
            {
                var result = await todoService.GetById(id);
                return Results.Ok(result);
            });

        todo.MapDelete("{id:int}", 
            async (ITodoService todoService, int id) =>
            {
                await todoService.Delete(id);
                return Results.Ok();
            });
    }
}
