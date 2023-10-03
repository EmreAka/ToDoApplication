using WebAPI.Security;

namespace WebAPI.Models;

public class ToDo : IEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; } = false;
    public virtual User User { get; set; }
}
