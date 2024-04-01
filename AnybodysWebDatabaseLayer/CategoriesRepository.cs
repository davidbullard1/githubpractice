using AnybodysWebDBLibrary;
using AnybodysWebModels;
using Microsoft.EntityFrameworkCore;

namespace AnybodysWebDatabaseLayer;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly AnybodysDbContext _context;
    public CategoriesRepository(AnybodysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetAsync(int id)
    {
        return await _context.Categories.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> AddOrUpdateAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category, "no way jose");

        if (category.Id > 0)
        {
            return await Update(category);
        }
        else
        {
            return await Add(category);
        }
    }

    private async Task<int> Add(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }

    private async Task<int> Update(Category category)
    {
        var existing = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
        if (existing is null)
        {
            throw new Exception("Not Found");
        }
        existing.Name = category.Name;
        await _context.SaveChangesAsync();
        return existing.Id;
    }

    public async Task<int> DeleteAsync(Category item)
    {
        return await DeleteAsync(item.Id);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var existing = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (existing is null)
        {
            throw new Exception("Not Found");
        }
        _context.Remove(existing);
        await _context.SaveChangesAsync();
        return existing.Id;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(x => x.Id == id);
    }
}
