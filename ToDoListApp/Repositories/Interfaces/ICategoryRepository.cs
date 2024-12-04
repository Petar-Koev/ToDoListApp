using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
