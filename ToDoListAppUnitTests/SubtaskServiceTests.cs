using Moq;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services;

namespace ToDoListAppUnitTests
{
    public class SubtaskServiceTests
    {
        private Mock<ISubtaskRepository> _mockSubtaskRepository;
        private SubtaskService _subtaskService;

        [SetUp]
        public void Setup()
        {
            _mockSubtaskRepository = new Mock<ISubtaskRepository>();
            _subtaskService = new SubtaskService(_mockSubtaskRepository.Object);
        }

        [Test]
        public async Task GetSubtasksByTodoIdAsync_ShouldReturnSubtaskViewModels_WhenSubtasksExist()
        {
            // Arrange
            int todoId = 1;
            var subtasks = new List<Subtask>
            {       
                new Subtask { Id = 1, Name = "Subtask 1", IsCompleted = false },
                new Subtask { Id = 2, Name = "Subtask 2", IsCompleted = true }
            };

            _mockSubtaskRepository
                .Setup(repo => repo.GetSubtasksByTodoIdAsync(todoId))
                .ReturnsAsync(subtasks);

            // Act
            var result = await _subtaskService.GetSubtasksByTodoIdAsync(todoId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual("Subtask 1", result[0].Name);
            ClassicAssert.IsFalse(result[0].IsCompleted);
            ClassicAssert.AreEqual("Subtask 2", result[1].Name);
            ClassicAssert.IsTrue(result[1].IsCompleted);

            _mockSubtaskRepository.Verify(repo => repo.GetSubtasksByTodoIdAsync(todoId), Times.Once);
        }

        [Test]
        public async Task GetSubtasksByTodoIdAsync_ShouldReturnEmptyList_WhenNoSubtasksExist()
        {
            // Arrange
            int todoId = 1;

            _mockSubtaskRepository
                .Setup(repo => repo.GetSubtasksByTodoIdAsync(todoId))
                .ReturnsAsync(new List<Subtask>());

            // Act
            var result = await _subtaskService.GetSubtasksByTodoIdAsync(todoId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(0, result.Count);

            _mockSubtaskRepository.Verify(repo => repo.GetSubtasksByTodoIdAsync(todoId), Times.Once);
        }

        [Test]
        public void GetSubtasksByTodoIdAsync_ShouldThrowNotFoundException_WhenSubtasksDoNotExist()
        {
            // Arrange
            int todoId = 1;

            _mockSubtaskRepository
                .Setup(repo => repo.GetSubtasksByTodoIdAsync(todoId))
                .ReturnsAsync((List<Subtask>?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _subtaskService.GetSubtasksByTodoIdAsync(todoId);
            });

            ClassicAssert.AreEqual($"Subtasks for ToDo with ID {todoId} not found.", ex.Message);
            _mockSubtaskRepository.Verify(repo => repo.GetSubtasksByTodoIdAsync(todoId), Times.Once);
        }

        [Test]
        public void SaveSubtasksAsync_ShouldThrowArgumentException_WhenTaskNameIsEmpty()
        {
            // Arrange
            int todoId = 1;
            var tasks = new List<SubtaskViewModel>
            {
                new SubtaskViewModel { Id = 0, Name = "", IsCompleted = false }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _subtaskService.SaveSubtasksAsync(todoId, tasks);
            });

            ClassicAssert.AreEqual("Task name cannot be empty.", ex.Message);
        }

        [Test]
        public void SaveSubtasksAsync_ShouldThrowNotFoundException_WhenSubtasksDoNotExist()
        {
            // Arrange
            int todoId = 1;
            var tasks = new List<SubtaskViewModel>
            {
                 new SubtaskViewModel { Id = 0, Name = "New Subtask", IsCompleted = false }
            };

            _mockSubtaskRepository
                .Setup(repo => repo.GetSubtasksByTodoIdAsync(todoId))
                .ReturnsAsync((List<Subtask>?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _subtaskService.SaveSubtasksAsync(todoId, tasks);
            });

            ClassicAssert.AreEqual($"Subtasks for ToDo with ID {todoId} not found.", ex.Message);
            _mockSubtaskRepository.Verify(repo => repo.GetSubtasksByTodoIdAsync(todoId), Times.Once);
        }

        [Test]
        public async Task SaveSubtasksAsync_ShouldAddAndUpdateSubtasks_WhenTasksAreValid()
        {
            // Arrange
            int todoId = 1;
            var tasks = new List<SubtaskViewModel>
            {
                new SubtaskViewModel { Id = 1, Name = "Existing Subtask", IsCompleted = true },
                new SubtaskViewModel { Id = 0, Name = "New Subtask", IsCompleted = false }
            };

            var existingSubtasks = new List<Subtask>
            {
                new Subtask { Id = 1, Name = "Existing Subtask", IsCompleted = false }
            };

            _mockSubtaskRepository
                .Setup(repo => repo.GetSubtasksByTodoIdAsync(todoId))
                .ReturnsAsync(existingSubtasks);

            _mockSubtaskRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Subtask>()))
                .Returns(Task.CompletedTask);

            _mockSubtaskRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _mockSubtaskRepository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _subtaskService.SaveSubtasksAsync(todoId, tasks);

            // Assert
            _mockSubtaskRepository.Verify(repo => repo.GetSubtasksByTodoIdAsync(todoId), Times.Once);
            _mockSubtaskRepository.Verify(repo => repo.AddAsync(It.Is<Subtask>(s => s.Name == "New Subtask")), Times.Once);
            _mockSubtaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void SaveSubtasksAsync_ShouldThrowArgumentException_WhenDuplicateTaskNamesExist()
        {
            // Arrange
            int todoId = 1;
            var tasks = new List<SubtaskViewModel>
            {
                 new SubtaskViewModel { Id = 0, Name = "Duplicate Task", IsCompleted = false },
                 new SubtaskViewModel { Id = 0, Name = "Duplicate Task", IsCompleted = true }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _subtaskService.SaveSubtasksAsync(todoId, tasks);
            });

            ClassicAssert.AreEqual("Duplicate tasks detected", ex.Message);
        }

        [Test]
        public void SaveSubtasksAsync_ShouldThrowArgumentException_WhenTaskNameIsNotRightLength()
        {
            // Arrange
            int todoId = 1;
            var tasks = new List<SubtaskViewModel>
            {
                 new SubtaskViewModel { Id = 0, Name = "D", IsCompleted = false },
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _subtaskService.SaveSubtasksAsync(todoId, tasks);
            });

            ClassicAssert.AreEqual($"Task name '{tasks[0].Name}' must be between 2 and 100 characters.", ex.Message);
        }







    }
}
