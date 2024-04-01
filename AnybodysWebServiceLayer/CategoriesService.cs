using AnybodysWebDatabaseLayer;
using AnybodysWebModels;

namespace AnybodysWebServiceLayer;

public class CategoriesService : ICategoriesService
{
    private ICategoriesRepository _categoriesRepository;

    public CategoriesService(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _categoriesRepository.GetAllAsync();
    }

    public Task<Category?> GetAsync(int id)
    {
        return _categoriesRepository.GetAsync(id);
    }

    public Task<int> AddOrUpdateAsync(Category category)
    {
        return _categoriesRepository.AddOrUpdateAsync(category);
    }

    public Task<int> DeleteAsync(Category category)
    {
        return _categoriesRepository.DeleteAsync(category);
    }

    public Task<int> DeleteAsync(int id)
    {
        return _categoriesRepository.DeleteAsync(id);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _categoriesRepository.ExistsAsync(id);
    }
}
