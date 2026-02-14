using LE_ToDo.BusinessLogic.Models;

namespace LE_ToDo.BusinessLogic.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItemDto>> GetAllAsync();
    Task<TodoItemDto> AddAsync(CreateTodoRequest request);
    Task<bool> DeleteAsync(int id);
}
