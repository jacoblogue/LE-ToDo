using LE_ToDo.DataAccess.Entities;

namespace LE_ToDo.DataAccess.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> AddAsync(TodoItem item);
    Task<bool> DeleteAsync(int id);
}
