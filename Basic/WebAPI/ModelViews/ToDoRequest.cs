namespace WebAPI.ModelViews;

public class ToDoRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
}
