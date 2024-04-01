using AnybodysWebDBLibrary;
using AnybodysWebModels;
using Microsoft.EntityFrameworkCore;

namespace AnybodysWebDatabaseLayer;

public class ItemsRepository : IItemsRepository
{
    private readonly AnybodysDbContext _context;

    public ItemsRepository(AnybodysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items.Include(x => x.Category).ToListAsync();
    }

    public async Task<Item?> GetAsync(int id)
    {
        return await _context.Items.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> AddOrUpdateAsync(Item item)
    {
        //make sure not null
        if (item is null)
        { 
            throw new ArgumentNullException("no way jose");
        }

        //make sure id is valid
        if (item.Id < 0)
        {
            throw new ArgumentOutOfRangeException("Invalid Id");
        }

        //make sure category is valid
        var goodCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Id == item.CategoryId);
        if (goodCategory is null)
        {
            throw new ArgumentOutOfRangeException("Category not found");
        }   

        //add or update
        if (item.Id > 0)
        {
            return await Update(item);
        }
        else
        {
            return await Add(item);
        }
    }

    private async Task<int> Add(Item item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        return item.Id;
    }

    private async Task<int> Update(Item item)
    {
        var existing = await _context.Items.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (existing is null)
        {
            throw new Exception("Not Found");
        }
        existing.Name = item.Name;
        existing.CategoryId = item.CategoryId;
        await _context.SaveChangesAsync();
        return existing.Id;
    }

    public async Task<int> DeleteAsync(Item item)
    {
        ArgumentNullException.ThrowIfNull(item, "Item cannot be null");
        return await DeleteAsync(item.Id);
    }

    public async Task<int> DeleteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id, "Invalid Id");
        ArgumentOutOfRangeException.ThrowIfZero(id, "Invalid Id");

        var existing = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
        
        ArgumentNullException.ThrowIfNull(existing, "Not Found");
        
        _context.Items.Remove(existing);
        await _context.SaveChangesAsync();
        return existing.Id; 
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Items.AnyAsync(x => x.Id == id);
    }
}
