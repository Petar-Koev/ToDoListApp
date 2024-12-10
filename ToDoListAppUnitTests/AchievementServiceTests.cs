using Moq;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services;

namespace ToDoListAppUnitTests
{
    public class AchievementServiceTests
    {
        private Mock<IAchievementRepository> _mockAchievementRepository;
        private AchievementService _achievementService;

        [SetUp]
        public void Setup()
        {
            _mockAchievementRepository = new Mock<IAchievementRepository>();
            _achievementService = new AchievementService(_mockAchievementRepository.Object);
        }

        [Test]
        public async Task GetAvailableAchievementsAsync_ShouldReturnAchievements_WhenAchievementsExist()
        {
            // Arrange
            string userId = "user123";
            var achievements = new List<Achievement>
            {
                new Achievement { Id = 1, Name = "First List", Description = "Create your first list" },
                new Achievement { Id = 2, Name = "First Todo", Description = "Add your first todo" },
                new Achievement { Id = 3, Name = "Complete a List", Description = "Complete all tasks in a list" },
                new Achievement { Id = 4, Name = "Complete Todo", Description = "Complete your first todo by checking all subtasks" },
                new Achievement { Id = 5, Name = "Complete 50 Todos Overall", Description = "Complete 50 todos in all lists combined" }
            };

            _mockAchievementRepository
                .Setup(repo => repo.GetAvailableAchievementsAsync(userId))
                .ReturnsAsync(achievements);

            // Act
            var result = await _achievementService.GetAvailableAchievementsAsync(userId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(5, result.Count);
            ClassicAssert.AreEqual("First List", result[0].Name);
            ClassicAssert.AreEqual("First Todo", result[1].Name);
            ClassicAssert.AreEqual("Complete a List", result[2].Name);
            ClassicAssert.AreEqual("Complete Todo", result[3].Name);
            ClassicAssert.AreEqual("Complete 50 Todos Overall", result[4].Name);
            _mockAchievementRepository.Verify(repo => repo.GetAvailableAchievementsAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetAvailableAchievementsAsync_ShouldReturnEmptyList_WhenNoAchievementsExist()
        {
            // Arrange
            string userId = "user123";

            _mockAchievementRepository
                .Setup(repo => repo.GetAvailableAchievementsAsync(userId))
                .ReturnsAsync(new List<Achievement>());

            // Act
            var result = await _achievementService.GetAvailableAchievementsAsync(userId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(0, result.Count);
            _mockAchievementRepository.Verify(repo => repo.GetAvailableAchievementsAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserAchievementsAsync_ShouldReturnAchievements_WhenAchievementsExist()
        {
            // Arrange
            string userId = "user123";
            var achievements = new List<Achievement>
            {
                 new Achievement { Id = 1, Name = "First List", Description = "Create your first list" },
            };

            _mockAchievementRepository
                .Setup(repo => repo.GetUserAchievementsAsync(userId))
                .ReturnsAsync(achievements);

            // Act
            var result = await _achievementService.GetUserAchievementsAsync(userId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(1, result.Count);
            ClassicAssert.AreEqual("First List", result[0].Name);
            _mockAchievementRepository.Verify(repo => repo.GetUserAchievementsAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserAchievementsAsync_ShouldReturnEmptyList_WhenNoAchievementsExist()
        {
            // Arrange
            string userId = "user123";

            _mockAchievementRepository
                .Setup(repo => repo.GetUserAchievementsAsync(userId))
                .ReturnsAsync(new List<Achievement>());

            // Act
            var result = await _achievementService.GetUserAchievementsAsync(userId);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(0, result.Count);
            _mockAchievementRepository.Verify(repo => repo.GetUserAchievementsAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetAchievementByNameAsync_ShouldReturnAchievement_WhenAchievementExists()
        {
            // Arrange
            string achievementName = "First List";
            var expectedAchievement = new Achievement
            {
                Id = 1,
                Name = achievementName,
                Description = "Create your first list"
            };

            _mockAchievementRepository
                .Setup(repo => repo.GetAchievementByNameAsync(achievementName))
                .ReturnsAsync(expectedAchievement);

            // Act
            var result = await _achievementService.GetAchievementByNameAsync(achievementName);

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(expectedAchievement.Id, result?.Id);
            ClassicAssert.AreEqual(expectedAchievement.Name, result?.Name);
            ClassicAssert.AreEqual(expectedAchievement.Description, result?.Description);
            _mockAchievementRepository.Verify(repo => repo.GetAchievementByNameAsync(achievementName), Times.Once);
        }

        [Test]
        public async Task GetAchievementByNameAsync_ShouldReturnNull_WhenAchievementDoesNotExist()
        {
            // Arrange
            string achievementName = "Nonexistent Achievement";

            _mockAchievementRepository
                .Setup(repo => repo.GetAchievementByNameAsync(achievementName))
                .ReturnsAsync((Achievement?)null);

            // Act
            var result = await _achievementService.GetAchievementByNameAsync(achievementName);

            // Assert
            ClassicAssert.IsNull(result);
            _mockAchievementRepository.Verify(repo => repo.GetAchievementByNameAsync(achievementName), Times.Once);
        }

        [Test]
        public async Task AwardAchievementAsync_ShouldAwardAchievement_WhenUserDoesNotAlreadyHaveIt()
        {
            // Arrange
            string userId = "user123";
            int achievementId = 1;
            var userAchievements = new List<Achievement>
            {
                new Achievement { Id = 2, Name = "Another Achievement" }
            };

            _mockAchievementRepository
                .Setup(repo => repo.GetUserAchievementsAsync(userId))
                .ReturnsAsync(userAchievements);

            _mockAchievementRepository
                .Setup(repo => repo.AddUserAchievementAsync(It.IsAny<UserAchievement>()))
                .Returns(Task.CompletedTask);

            // Act
            await _achievementService.AwardAchievementAsync(userId, achievementId);

            // Assert
            _mockAchievementRepository.Verify(repo => repo.GetUserAchievementsAsync(userId), Times.Once);
            _mockAchievementRepository.Verify(repo => repo.AddUserAchievementAsync(It.Is<UserAchievement>(ua =>
                ua.UserId == userId && ua.AchievementId == achievementId
            )), Times.Once);
        }

        [Test]
        public async Task AwardAchievementAsync_ShouldDoNothing_WhenUserAlreadyHasTheAchievement()
        {
            // Arrange
            string userId = "user123";
            int achievementId = 1;
            var userAchievements = new List<Achievement>
    {
        new Achievement { Id = achievementId, Name = "Existing Achievement" }
    };

            _mockAchievementRepository
                .Setup(repo => repo.GetUserAchievementsAsync(userId))
                .ReturnsAsync(userAchievements);

            // Act
            await _achievementService.AwardAchievementAsync(userId, achievementId);

            // Assert
            _mockAchievementRepository.Verify(repo => repo.GetUserAchievementsAsync(userId), Times.Once);
            _mockAchievementRepository.Verify(repo => repo.AddUserAchievementAsync(It.IsAny<UserAchievement>()), Times.Never);
        }








    }
}
