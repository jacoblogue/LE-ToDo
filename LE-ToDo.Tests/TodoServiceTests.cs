using LE_ToDo.BusinessLogic.Models;
using LE_ToDo.BusinessLogic.Services;
using LE_ToDo.DataAccess.Entities;
using LE_ToDo.DataAccess.Repositories;
using Moq;

namespace LE_ToDo.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepo;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _mockRepo = new Mock<ITodoRepository>();
        _service = new TodoService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = 1, Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Task 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Task 1", result[0].Title);
        Assert.Equal("Task 2", result[1].Title);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsCreatedItem()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "New Task" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) =>
            {
                item.Id = 1;
                return item;
            });

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("New Task", result.Title);
        Assert.False(result.IsCompleted);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_EmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "   " };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(request));
    }

    [Fact]
    public async Task AddAsync_TrimsTitle()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "  Trimmed Task  " };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) =>
            {
                item.Id = 1;
                return item;
            });

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        Assert.Equal("Trimmed Task", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
}
