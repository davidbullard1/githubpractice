using AnybodysWebModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnybodysWebServiceLayer;

public interface IItemsService
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetAsync(int id);
    Task<int> AddOrUpdateAsync(Item item);
    Task<int> DeleteAsync(Item item);
    Task<int> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
