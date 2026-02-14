using LE_ToDo.BusinessLogic.Models;
using LE_ToDo.DataAccess.Entities;
using LE_ToDo.DataAccess.Repositories;

namespace LE_ToDo.BusinessLogic.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<TodoItemDto> AddAsync(CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title cannot be empty.", nameof(request));

        var entity = new TodoItem
        {
            Title = request.Title.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static TodoItemDto MapToDto(TodoItem entity)
    {
        return new TodoItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            IsCompleted = entity.IsCompleted,
            CreatedAt = entity.CreatedAt
        };
    }
}
