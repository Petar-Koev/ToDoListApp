using Moq;
using NUnit.Framework.Legacy;
using ToDoListApp.Constants;
using ToDoListApp.Data;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services;
using ToDoListApp.Services.Interfaces;

namespace ToDoListAppUnitTests
{
    public class ToDoListServiceTests
    {
        private Mock<IToDoListRepository> _mockToDoListRepository;
        private Mock<IAchievementService> _mockAchievementService;
        private ToDoListService _toDoListService;

        [SetUp]
        public void Setup()
        {
            _mockToDoListRepository = new Mock<IToDoListRepository>();
            _mockAchievementService = new Mock<IAchievementService>();
            _toDoListService = new ToDoListService(_mockToDoListRepository.Object, _mockAchievementService.Object);
        }

        [Test]
        public async Task GetListsForUserAsync_ReturnsCorrectViewModel()
        {
            // Arrange
            string userId = "test-user";
            var mockToDoLists = new List<ToDoList>
        {
            new ToDoList { Id = 1, Name = "List1", Category = new Category { Name = "Work" }, UserId = userId },
            new ToDoList { Id = 2, Name = "List2", Category = new Category { Name = "Personal" }, UserId = userId }
        };

            _mockToDoListRepository
                .Setup(repo => repo.GetListsByUserAsync(userId))
                .ReturnsAsync(mockToDoLists);

            // Act
            var result = await _toDoListService.GetListsForUserAsync(userId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count);

            ClassicAssert.AreEqual(1, result[0].Id);
            ClassicAssert.AreEqual("List1", result[0].Name);
            ClassicAssert.AreEqual("Work", result[0].CategoryName);
            ClassicAssert.AreEqual(2, result[1].Id);
            ClassicAssert.AreEqual("List2", result[1].Name);
            ClassicAssert.AreEqual("Personal", result[1].CategoryName);

            _mockToDoListRepository.Verify(repo => repo.GetListsByUserAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetListsForUserAsync_ShouldReturnEmptyList_WhenNoDataFound()
        {
            // Arrange
            string userId = "test-user";
            _mockToDoListRepository
                .Setup(repo => repo.GetListsByUserAsync(userId))
                .ReturnsAsync(new List<ToDoList>());

            // Act
            var result = await _toDoListService.GetListsForUserAsync(userId);

            // Assert
            ClassicAssert.NotNull(result); 
            ClassicAssert.IsEmpty(result); 
        }

        [Test]
        public async Task AddListAsync_ShouldAddList_WhenNameIsNotDuplicate()
        {
            // Arrange
            var userId = "test-user";
            var listViewModel = new ToDoListViewModel
            {
                Name = "New List",
                Description = "Description",
                CategoryId = 1
            };

            _mockToDoListRepository
                .Setup(repo => repo.ListNameExistsAsync(listViewModel.Name, userId))
                .ReturnsAsync(false);

            _mockToDoListRepository
                .Setup(repo => repo.AddListAsync(It.IsAny<ToDoList>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoListService.AddListAsync(listViewModel, userId);

            // Assert
            _mockToDoListRepository.Verify(repo => repo.ListNameExistsAsync(listViewModel.Name, userId), Times.Once);
            _mockToDoListRepository.Verify(repo => repo.AddListAsync(It.IsAny<ToDoList>()), Times.Once);
        }

        [Test]
        public void AddListAsync_ShouldThrowException_WhenNameIsDuplicate()
        {
            // Arrange
            var userId = "test-user";
            var listViewModel = new ToDoListViewModel
            {
                Name = "Duplicate List",
                Description = "Description",
                CategoryId = 1
            };

            _mockToDoListRepository
                .Setup(repo => repo.ListNameExistsAsync(listViewModel.Name, userId))
                .ReturnsAsync(true);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _toDoListService.AddListAsync(listViewModel, userId);
            }, ErrorMessages.DuplicateListName);

            _mockToDoListRepository.Verify(repo => repo.ListNameExistsAsync(listViewModel.Name, userId), Times.Once);
            _mockToDoListRepository.Verify(repo => repo.AddListAsync(It.IsAny<ToDoList>()), Times.Never);
        }

        [Test]
        public async Task SaveAchievementAsync_ShouldAwardAchievement_WhenNoOtherListsAndAchievementExists()
        {
            // Arrange
            var userId = "test-user";
            var achievementName = "First List";
            var achievementId = 1;

            _mockToDoListRepository
                .Setup(repo => repo.HasListsAsync(userId))
                .ReturnsAsync(false);

            _mockAchievementService
                .Setup(service => service.GetAchievementByNameAsync(achievementName))
                .ReturnsAsync(new Achievement { Id = achievementId });

            _mockAchievementService
                .Setup(service => service.AwardAchievementAsync(userId, achievementId))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoListService.SaveAchievementAsync(userId);

            // Assert
            _mockToDoListRepository.Verify(repo => repo.HasListsAsync(userId), Times.Once);
            _mockAchievementService.Verify(service => service.GetAchievementByNameAsync(achievementName), Times.Once);
            _mockAchievementService.Verify(service => service.AwardAchievementAsync(userId, achievementId), Times.Once);
        }

        [Test]
        public async Task SaveAchievementAsync_ShouldNotAwardAchievement_WhenAchievementDoesNotExist()
        {
            // Arrange
            var userId = "test-user";
            var achievementName = "First List";

            _mockToDoListRepository
                .Setup(repo => repo.HasListsAsync(userId))
                .ReturnsAsync(false);

            _mockAchievementService
                .Setup(service => service.GetAchievementByNameAsync(achievementName))
                .ReturnsAsync((Achievement?)null);

            // Act
            await _toDoListService.SaveAchievementAsync(userId);

            // Assert
            _mockToDoListRepository.Verify(repo => repo.HasListsAsync(userId), Times.Once);
            _mockAchievementService.Verify(service => service.GetAchievementByNameAsync(achievementName), Times.Once);
            _mockAchievementService.Verify(service => service.AwardAchievementAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }


        [Test]
        public async Task SaveAchievementAsync_ShouldNotAwardAchievement_WhenUserHasOtherLists()
        {
            // Arrange
            var userId = "test-user";

            _mockToDoListRepository
                .Setup(repo => repo.HasListsAsync(userId))
                .ReturnsAsync(true);

            // Act
            await _toDoListService.SaveAchievementAsync(userId);

            // Assert
            _mockToDoListRepository.Verify(repo => repo.HasListsAsync(userId), Times.Once);
            _mockAchievementService.Verify(service => service.GetAchievementByNameAsync(It.IsAny<string>()), Times.Never);
            _mockAchievementService.Verify(service => service.AwardAchievementAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetListByIdAsync_ShouldReturnList_WhenListExists()
        {
            // Arrange
            var listId = 1;
            var expectedList = new ToDoList { Id = listId, Name = "Sample List" };
            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _toDoListService.GetListByIdAsync(listId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(expectedList.Id, result.Id);
            ClassicAssert.AreEqual(expectedList.Name, result.Name);
        }

        [Test]
        public void GetListByIdAsync_ShouldThrowNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var listId = 1;
            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync((ToDoList?)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoListService.GetListByIdAsync(listId);
            }, $"List with ID {listId} not found.");
        }

        [Test]
        public async Task UpdateListAsync_ShouldUpdateList_WhenValidInputProvided()
        {
            // Arrange
            var listViewModel = new ToDoListViewModel
            {
                Id = 1,
                Name = "Updated List",
                Description = "Updated Description",
                CategoryId = 2
            };

            var existingList = new ToDoList
            {
                Id = 1,
                Name = "Old List",
                Description = "Old Description",
                CategoryId = 1
            };

            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listViewModel.Id.Value))
                .ReturnsAsync(existingList);

            _mockToDoListRepository
                .Setup(repo => repo.UpdateListAsync(It.IsAny<ToDoList>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoListService.UpdateListAsync(listViewModel);

            // Assert
            _mockToDoListRepository.Verify(repo => repo.GetListByIdAsync(listViewModel.Id.Value), Times.Once);
            _mockToDoListRepository.Verify(repo => repo.UpdateListAsync(It.Is<ToDoList>(list =>
                list.Id == listViewModel.Id &&
                list.Name == listViewModel.Name &&
                list.Description == listViewModel.Description &&
                list.CategoryId == listViewModel.CategoryId
            )), Times.Once);
        }

        [Test]
        public void UpdateListAsync_ShouldThrowArgumentException_WhenIdIsNull()
        {
            // Arrange
            var listViewModel = new ToDoListViewModel
            {
                Id = null,
                Name = "New List",
                Description = "Description",
                CategoryId = 1
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _toDoListService.UpdateListAsync(listViewModel);
            });

            ClassicAssert.AreEqual(ErrorMessages.InvalidListId, ex.Message);
        }

        [Test]
        public async Task DeleteListAsync_ShouldMarkListAsDeleted_WhenListExists()
        {
            // Arrange
            var listId = 1;
            var existingList = new ToDoList
            {
                Id = listId,
                Name = "Sample List",
                IsDeleted = false
            };

            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync(existingList);

            _mockToDoListRepository
                .Setup(repo => repo.UpdateListAsync(It.IsAny<ToDoList>()))
                .Returns(Task.CompletedTask);

            // Act
            await _toDoListService.DeleteListAsync(listId);

            // Assert
            ClassicAssert.IsTrue(existingList.IsDeleted);
            _mockToDoListRepository.Verify(repo => repo.GetListByIdAsync(listId), Times.Once);
            _mockToDoListRepository.Verify(repo => repo.UpdateListAsync(It.Is<ToDoList>(list =>
                list.Id == listId && list.IsDeleted == true
            )), Times.Once);
        }

        [Test]
        public void DeleteListAsync_ShouldThrowNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var listId = 1;

            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync((ToDoList?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoListService.DeleteListAsync(listId);
            });

            ClassicAssert.AreEqual($"List with ID {listId} not found.", ex.Message);
            _mockToDoListRepository.Verify(repo => repo.GetListByIdAsync(listId), Times.Once);
            _mockToDoListRepository.Verify(repo => repo.UpdateListAsync(It.IsAny<ToDoList>()), Times.Never);
        }

        [Test]
        public async Task GetListNameByIdAsync_ShouldReturnListName_WhenListExists()
        {
            // Arrange
            var listId = 1;
            var expectedName = "Sample List";

            var existingList = new ToDoList
            {
                Id = listId,
                Name = expectedName
            };

            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync(existingList);

            // Act
            var result = await _toDoListService.GetListNameByIdAsync(listId);

            // Assert
            ClassicAssert.AreEqual(expectedName, result);
            _mockToDoListRepository.Verify(repo => repo.GetListByIdAsync(listId), Times.Once);
        }

        [Test]
        public void GetListNameByIdAsync_ShouldThrowNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var listId = 1;

            _mockToDoListRepository
                .Setup(repo => repo.GetListByIdAsync(listId))
                .ReturnsAsync((ToDoList?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _toDoListService.GetListNameByIdAsync(listId);
            });

            ClassicAssert.AreEqual($"List with ID {listId} not found.", ex.Message);
            _mockToDoListRepository.Verify(repo => repo.GetListByIdAsync(listId), Times.Once);
        }

        [Test]
        public async Task GetListCountByUserAsync_ShouldReturnCorrectCount_WhenUserHasLists()
        {
            // Arrange
            var userId = "test-user";
            var expectedCount = 5;

            _mockToDoListRepository
                .Setup(repo => repo.GetListCountByUserAsync(userId))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _toDoListService.GetListCountByUserAsync(userId);

            // Assert
            ClassicAssert.AreEqual(expectedCount, result);
            _mockToDoListRepository.Verify(repo => repo.GetListCountByUserAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetListCountByUserAsync_ShouldReturnZero_WhenUserHasNoLists()
        {
            // Arrange
            var userId = "test-user";

            _mockToDoListRepository
                .Setup(repo => repo.GetListCountByUserAsync(userId))
                .ReturnsAsync(0);

            // Act
            var result = await _toDoListService.GetListCountByUserAsync(userId);

            // Assert
            ClassicAssert.AreEqual(0, result);
            _mockToDoListRepository.Verify(repo => repo.GetListCountByUserAsync(userId), Times.Once);
        }
    }
}