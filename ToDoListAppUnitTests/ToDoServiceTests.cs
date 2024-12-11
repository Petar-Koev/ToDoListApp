using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;
using ToDoListApp.Services;
using ToDoListApp.Data;
using NUnit.Framework.Legacy;
using ToDoListApp.Enums;
using ToDoListApp.Models;
using ToDoListApp.Exceptions;
using ToDoListApp.Constants;

namespace ToDoListAppUnitTests
{
    public class ToDoServiceTests
    {
        private Mock<IToDoRepository> _mockRepository;
        private Mock<IToDoListService> _mockToDoListService;
        private ToDoService _toDoService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IToDoRepository>();
            _mockToDoListService = new Mock<IToDoListService>();
            _toDoService = new ToDoService(_mockRepository.Object, _mockToDoListService.Object);
        }

        [Test]
        public async Task GetTodosByListIdAsync_ShouldReturnTodosWithoutSorting_WhenSortByPriorityIsFalse()
        {
            // Arrange
            int listId = 1;
            var todos = new List<ToDo>
            {
                new ToDo
                {
                Id = 1,
                Name = "Task 1",
                Priority = Priority.Medium,
                IsCompleted = false,
                Subtasks = new List<Subtask>
                {
                    new Subtask { Id = 1, Name = "Subtask 1", IsCompleted = false },
                    new Subtask { Id = 2, Name = "Subtask 2", IsCompleted = true }
                }
                },
                 new ToDo
                 {
                 Id = 2,
                 Name = "Task 2",
                 Priority = Priority.Low,
                 IsCompleted = true,
                 Subtasks = new List<Subtask>()
                 }
            };

            _mockRepository
                .Setup(repo => repo.GetTodosByListIdAsync(listId))
                .ReturnsAsync(todos);

            _mockToDoListService
                .Setup(service => service.GetListNameByIdAsync(listId))
                .ReturnsAsync("Test List");

            // Act
            var result = await _toDoService.GetTodosByListIdAsync(listId, sortByPriority: false);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(listId, result.ListId);
            ClassicAssert.AreEqual("Test List", result.ListName);
            ClassicAssert.AreEqual(2, result.Todos.Count());

            var firstTodo = result.Todos.First();
            ClassicAssert.AreEqual(1, firstTodo.Id);
            ClassicAssert.AreEqual("Task 1", firstTodo.Name);
            ClassicAssert.AreEqual(false, firstTodo.IsChecked);
            ClassicAssert.AreEqual(2, firstTodo.Subtasks.Count());
            ClassicAssert.AreEqual("Subtask 1", firstTodo.Subtasks.First().Name);
        }

        [Test]
        public async Task GetTodosByListIdAsync_ShouldReturnTodosSortedByPriority_WhenSortByPriorityIsTrue()
        {
            // Arrange
            int listId = 1;
            var todos = new List<ToDo>
             {
            new ToDo
            {
                Id = 1,
                Name = "Task 1",
                Priority = Priority.Low,
                IsCompleted = false,
                Subtasks = new List<Subtask>()
            },
            new ToDo
            {
                Id = 2,
                Name = "Task 2",
                Priority = Priority.High,
                IsCompleted = true,
                Subtasks = new List<Subtask>()
            }
             };

            _mockRepository
                .Setup(repo => repo.GetTodosByListIdAsync(listId))
                .ReturnsAsync(todos);

            _mockToDoListService
                .Setup(service => service.GetListNameByIdAsync(listId))
                .ReturnsAsync("Test List");

            // Act
            var result = await _toDoService.GetTodosByListIdAsync(listId, sortByPriority: true);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(listId, result.ListId);
            ClassicAssert.AreEqual("Test List", result.ListName);
            ClassicAssert.AreEqual(2, result.Todos.Count());

            var firstTodo = result.Todos.First();
            ClassicAssert.AreEqual(2, firstTodo.Id);
            ClassicAssert.AreEqual("Task 2", firstTodo.Name); 
        }

        [Test]
        public async Task AddToDoAsync_ShouldAddToDo_WhenInputIsValid()
        {
            // Arrange
            var model = new AddToDoViewModel
            {
                Name = "Test ToDo",
                DueDate = DateTime.Now.AddDays(2),
                Priority = Priority.High,
                ListId = 1,
                Tasks = new List<TaskViewModel>
                {
                    new TaskViewModel { Name = "Subtask 1" },
                    new TaskViewModel { Name = "Subtask 2" }
                }
            };

            _mockToDoListService
                .Setup(service => service.GetListByIdAsync(model.ListId))
                .ReturnsAsync(new ToDoList { Id = model.ListId });

            _mockRepository
                .Setup(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId))
                .ReturnsAsync(false);

            _mockRepository
                .Setup(repo => repo.AddToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.AddToDoAsync(model);

            // Assert
            _mockToDoListService.Verify(service => service.GetListByIdAsync(model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.AddToDoAsync(It.Is<ToDo>(todo =>
                todo.Name == model.Name &&
                todo.DueDate == model.DueDate &&
                todo.Priority == model.Priority &&
                todo.ListId == model.ListId &&
                todo.Subtasks.Count == model.Tasks.Count &&
                todo.Subtasks.First().Name == "Subtask 1"
            )), Times.Once);
        }

        [Test]
        public void AddToDoAsync_ShouldThrowException_WhenDuplicateNameExists()
        {
            // Arrange
            var model = new AddToDoViewModel
            {
                Name = "Test ToDo",
                DueDate = DateTime.Now.AddDays(2),
                Priority = Priority.High,
                ListId = 1,
                Tasks = new List<TaskViewModel>()
            };

            _mockToDoListService
                .Setup(service => service.GetListByIdAsync(model.ListId))
                .ReturnsAsync(new ToDoList { Id = model.ListId });

            _mockRepository
                .Setup(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId))
                .ReturnsAsync(true);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _toDoService.AddToDoAsync(model);
            });

            ClassicAssert.AreEqual(ErrorMessages.DuplicateToDoName, ex.Message);
            _mockToDoListService.Verify(service => service.GetListByIdAsync(model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.AddToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public void AddToDoAsync_ShouldThrowNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var model = new AddToDoViewModel
            {
                Name = "Test ToDo",
                DueDate = DateTime.Now.AddDays(2),
                Priority = Priority.High,
                ListId = 1,
                Tasks = new List<TaskViewModel>()
            };

            _mockToDoListService
                .Setup(service => service.GetListByIdAsync(model.ListId))
                .ThrowsAsync(new NotFoundException(string.Format(ErrorMessages.ListNotFound, model.ListId)));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.AddToDoAsync(model);
            });

            ClassicAssert.AreEqual(string.Format(ErrorMessages.ListNotFound, model.ListId), ex.Message);
            _mockToDoListService.Verify(service => service.GetListByIdAsync(model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _mockRepository.Verify(repo => repo.AddToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public async Task MarkAsCheckedAsync_ShouldMarkToDoAsChecked_WhenAllSubtasksAreCompleted()
        {
            // Arrange
            int todoId = 1;
            var todo = new ToDo
            {
                Id = todoId,
                Name = "Test ToDo",
                IsCompleted = false,
                Subtasks = new List<Subtask>
                {
                    new Subtask { Id = 1, Name = "Subtask 1", IsCompleted = true },
                    new Subtask { Id = 2, Name = "Subtask 2", IsCompleted = true }
                }
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync(todo);

            _mockRepository
                .Setup(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.MarkAsCheckedAsync(todoId);

            // Assert
            ClassicAssert.IsTrue(todo.IsCompleted);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.Is<ToDo>(t => t.IsCompleted)), Times.Once);
        }

        [Test]
        public void MarkAsCheckedAsync_ShouldThrowInvalidOperationException_WhenSubtasksAreNotCompleted()
        {
            // Arrange
            int todoId = 1;
            var todo = new ToDo
            {
                Id = todoId,
                Name = "Test ToDo",
                IsCompleted = false,
                Subtasks = new List<Subtask>
        {
            new Subtask { Id = 1, Name = "Subtask 1", IsCompleted = true },
            new Subtask { Id = 2, Name = "Subtask 2", IsCompleted = false }
        }
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync(todo);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _toDoService.MarkAsCheckedAsync(todoId);
            });

            ClassicAssert.AreEqual(ErrorMessages.UncompletedSubtasks, ex.Message);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public void MarkAsCheckedAsync_ShouldThrowNotFoundException_WhenToDoDoesNotExist()
        {
            // Arrange
            int todoId = 1;

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ThrowsAsync(new NotFoundException(string.Format(ErrorMessages.TodoNotFound, todoId)));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.MarkAsCheckedAsync(todoId);
            });

            ClassicAssert.AreEqual(string.Format(ErrorMessages.TodoNotFound, todoId), ex.Message);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public async Task MarkAsUncheckedAsync_ShouldMarkToDoAsUnchecked_WhenToDoExists()
        {
            // Arrange
            int todoId = 1;
            var todo = new ToDo
            {
                Id = todoId,
                Name = "Test ToDo",
                IsCompleted = true,
                Subtasks = new List<Subtask>()
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync(todo);

            _mockRepository
                .Setup(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.MarkAsUncheckedAsync(todoId);

            // Assert
            ClassicAssert.IsFalse(todo.IsCompleted); 
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.Is<ToDo>(t => !t.IsCompleted)), Times.Once);
        }

        [Test]
        public void MarkAsUncheckedAsync_ShouldThrowNotFoundException_WhenToDoDoesNotExist()
        {
            // Arrange
            int todoId = 1;

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ThrowsAsync(new NotFoundException(string.Format(ErrorMessages.TodoNotFound, todoId)));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.MarkAsUncheckedAsync(todoId);
            });

            ClassicAssert.AreEqual(string.Format(ErrorMessages.TodoNotFound, todoId), ex.Message);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public async Task UpdateToDoAsync_ShouldUpdateToDo_WhenNameIsNotChanged()
        {
            // Arrange
            var model = new EditToDoViewModel
            {
                Id = 1,
                Name = "Existing ToDo Name", 
                DueDate = DateTime.Now.AddDays(3),
                Priority = Priority.High,
                ListId = 1
            };

            var existingToDo = new ToDo
            {
                Id = model.Id,
                Name = "Existing ToDo Name", 
                DueDate = DateTime.Now.AddDays(1),
                Priority = Priority.Medium
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(model.Id))
                .ReturnsAsync(existingToDo);

            _mockRepository
                .Setup(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.UpdateToDoAsync(model);

            // Assert
            ClassicAssert.AreEqual(model.DueDate, existingToDo.DueDate);
            ClassicAssert.AreEqual(model.Priority, existingToDo.Priority);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.Is<ToDo>(todo =>
                todo.DueDate == model.DueDate &&
                todo.Priority == model.Priority
            )), Times.Once);
        }

        [Test]
        public async Task UpdateToDoAsync_ShouldUpdateToDo_WhenNameIsChangedAndNotDuplicate()
        {
            // Arrange
            var model = new EditToDoViewModel
            {
                Id = 1,
                Name = "New ToDo Name", 
                DueDate = DateTime.Now.AddDays(3),
                Priority = Priority.High,
                ListId = 1
            };

            var existingToDo = new ToDo
            {
                Id = model.Id,
                Name = "Old ToDo Name", 
                DueDate = DateTime.Now.AddDays(1),
                Priority = Priority.Medium
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(model.Id))
                .ReturnsAsync(existingToDo);

            _mockRepository
                .Setup(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId))
                .ReturnsAsync(false); 

            _mockRepository
                .Setup(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.UpdateToDoAsync(model);

            // Assert
            ClassicAssert.AreEqual(model.Name, existingToDo.Name);
            ClassicAssert.AreEqual(model.DueDate, existingToDo.DueDate);
            ClassicAssert.AreEqual(model.Priority, existingToDo.Priority);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.Is<ToDo>(todo =>
                todo.Name == model.Name &&
                todo.DueDate == model.DueDate &&
                todo.Priority == model.Priority
            )), Times.Once);
        }

        [Test]
        public void UpdateToDoAsync_ShouldThrowNotFoundException_WhenToDoDoesNotExist()
        {
            // Arrange
            var model = new EditToDoViewModel
            {
                Id = 1,
                Name = "Updated ToDo",
                DueDate = DateTime.Now.AddDays(3),
                Priority = Priority.High
            };

            _mockRepository
                .Setup(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId))
                .ReturnsAsync(false);

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(model.Id))
                .ThrowsAsync(new NotFoundException(string.Format(ErrorMessages.TodoNotFound, model.Id)));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.UpdateToDoAsync(model);
            });

            ClassicAssert.AreEqual(string.Format(ErrorMessages.TodoNotFound, model.Id), ex.Message);

            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId), Times.Never);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(model.Id), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public void UpdateToDoAsync_ShouldThrowArgumentException_WhenNameIsChangedAndDuplicateExists()
        {
            // Arrange
            var model = new EditToDoViewModel
            {
                Id = 1,
                Name = "Duplicate ToDo Name", 
                DueDate = DateTime.Now.AddDays(3),
                Priority = Priority.High,
                ListId = 1
            };

            var existingToDo = new ToDo
            {
                Id = model.Id,
                Name = "Old ToDo Name", 
                DueDate = DateTime.Now.AddDays(1),
                Priority = Priority.Medium
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(model.Id))
                .ReturnsAsync(existingToDo);

            _mockRepository
                .Setup(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId))
                .ReturnsAsync(true); 

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _toDoService.UpdateToDoAsync(model);
            });

            ClassicAssert.AreEqual(ErrorMessages.DuplicateToDoName, ex.Message);
            _mockRepository.Verify(repo => repo.ToDoNameExistsAsync(model.Name, model.ListId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }

        [Test]
        public async Task GetToDoByIdAsync_ShouldReturnToDo_WhenToDoExists()
        {
            // Arrange
            int todoId = 1;
            var expectedToDo = new ToDo
            {
                Id = todoId,
                Name = "Sample ToDo",
                Priority = Priority.High,
                IsCompleted = false
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync(expectedToDo);

            // Act
            var result = await _toDoService.GetToDoByIdAsync(todoId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(expectedToDo.Id, result.Id);
            ClassicAssert.AreEqual(expectedToDo.Name, result.Name);
            ClassicAssert.AreEqual(expectedToDo.Priority, result.Priority);
            ClassicAssert.AreEqual(expectedToDo.IsCompleted, result.IsCompleted);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
        }

        [Test]
        public void GetToDoByIdAsync_ShouldThrowNotFoundException_WhenToDoDoesNotExist()
        {
            // Arrange
            int todoId = 1;

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync((ToDo?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.GetToDoByIdAsync(todoId);
            });

            ClassicAssert.AreEqual(string.Format(ErrorMessages.TodoNotFound, todoId), ex.Message);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
        }

        [Test]
        public async Task DeleteToDoAsync_ShouldDeleteToDo_WhenToDoExists()
        {
            // Arrange
            int todoId = 1;
            var existingToDo = new ToDo
            {
                Id = todoId,
                Name = "Sample ToDo",
                IsCompleted = false
            };

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ReturnsAsync(existingToDo);

            _mockRepository
                .Setup(repo => repo.DeleteToDoAsync(It.IsAny<ToDo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoService.DeleteToDoAsync(todoId);

            // Assert
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteToDoAsync(It.Is<ToDo>(todo => todo.Id == todoId)), Times.Once);
        }

        [Test]
        public void DeleteToDoAsync_ShouldThrowNotFoundException_WhenToDoDoesNotExist()
        {
            // Arrange
            int todoId = 1;

            _mockRepository
                .Setup(repo => repo.GetToDoByIdAsync(todoId))
                .ThrowsAsync(new NotFoundException($"ToDo with ID {todoId} not found."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoService.DeleteToDoAsync(todoId);
            });

            ClassicAssert.AreEqual($"ToDo with ID {todoId} not found.", ex.Message);
            _mockRepository.Verify(repo => repo.GetToDoByIdAsync(todoId), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteToDoAsync(It.IsAny<ToDo>()), Times.Never);
        }
    }
}
