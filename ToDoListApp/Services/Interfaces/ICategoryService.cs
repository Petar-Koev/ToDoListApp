using ToDoListApp.Data;

namespace ToDoListApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
