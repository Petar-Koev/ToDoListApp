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
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
        }

        [Test]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Work" },
                    new Category { Id = 2, Name = "Personal" }
            };

            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual("Work", result[0].Name);
            ClassicAssert.AreEqual("Personal", result[1].Name);
            _mockCategoryRepository.Verify(repo => repo.GetAllCategoriesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(new List<Category>());

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(0, result.Count);
            _mockCategoryRepository.Verify(repo => repo.GetAllCategoriesAsync(), Times.Once);
        }


    }
}
