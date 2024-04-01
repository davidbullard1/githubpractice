using AnybodysWebModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnybodysWebDatabaseLayer;

public class CategoriesFakeDataRepository : ICategoriesRepository
{
    //some other database connection/orm stuff could go here
    private static List<Category> _categories;

    public CategoriesFakeDataRepository()
    {
        //any other database could go here

        if (_categories is null || !_categories.Any())
        {
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Star Wars" },
                new Category { Id = 2, Name = "Star Trek" },
                new Category { Id = 3, Name = "Other" }
            };
        }
        
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await Task.FromResult(_categories);
    }

    public async Task<Category?> GetAsync(int id)
    {
        return Task.FromResult(_categories.SingleOrDefault(x => x.Id == id)).Result;
    }

    public async Task<int> AddOrUpdateAsync(Category category)
    {
        if (category.Id > 0)
        {
            //do the update...
        }
        else
        {
            category.Id = _categories.Max(x => x.Id) + 1;
            _categories.Add(category);
        }
        
        return Task.FromResult(category.Id).Result;
    }

    public async Task<int> DeleteAsync(Category category)
    {
        return Task.FromResult(category.Id).Result;
    }

    public async Task<int> DeleteAsync(int id)
    {
        return Task.FromResult(id).Result;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_categories.Any(x => x.Id == id)).Result;
    }
}
