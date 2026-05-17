

using TodoList.Domain.common;

namespace TodoList.Domain.Entities;

public class User : BaseEntity<Guid> 
{
    public required string Name {get; set;}
    public required string Username { get; set; } 
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public ICollection<TodoItem> TodoItems { get; set; } = [];
    public User(){} 
}


