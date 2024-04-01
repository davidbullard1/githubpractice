using AnybodysWebDatabaseLayer;
using AnybodysWebDBLibrary;
using AnybodysWebModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AnybodysWebDatabaseLayer_Tests;

public class TestItemsRepository
{
    private ItemsRepository _repo;
    private DbContextOptions<AnybodysDbContext> _options;

    public TestItemsRepository()
    {
        SetupOptions();
        SeedData();
    }

    private void SetupOptions()
    {
        _options = new DbContextOptionsBuilder<AnybodysDbContext>()
                    .UseInMemoryDatabase(databaseName: "ItemsWebManagerDbTests")
                    .Options;
    }

    private List<Category> Categories()
    {
        //create at least two categories
        return new List<Category>(){
            new Category() { Id = 1, Name = "Food" },
            new Category() { Id = 2, Name = "Hotel" }
        };
    }

    private List<Item> Items()
    {
        //create at least three items
        return new List<Item>() {
            new Item() { Id = 1, CategoryId = 1, ImagePath = "Some Path", Name = "Hamburger" },
            new Item() { Id = 2, CategoryId = 1, ImagePath = "Some Path", Name = "Pizza" },
            new Item() { Id = 3, CategoryId = 2, ImagePath = "Some Path", Name = "Holiday Inn" }
        };
    }

    private void SeedData()
    {
        var cats = Categories();
        var items = Items();

        using (var context = new AnybodysDbContext(_options))
        {
            var existingCategories = Task.Run(() => context.Categories.ToListAsync()).Result;
            if (existingCategories is null || existingCategories.Count == 0)
            {
                context.Categories.AddRange(cats);
                context.SaveChanges();
            }

            var existingItems = Task.Run(() => context.Items.ToListAsync()).Result;
            if (existingItems is null || existingItems.Count == 0)
            {
                context.Items.AddRange(items);
                context.SaveChanges();
            }
        }

    }

    [Fact]
    public async Task TestGetAllItems()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            var items = await _repo.GetAllAsync();

            //Assert
            Assert.NotNull(items);
            items.ShouldNotBeNull();

            items.Count.ShouldBe(3, "You didn't make the items or something...");
        }
    }

    [Theory]
    [InlineData(1, "Hamburger", 1)]
    [InlineData(2, "Pizza", 1)]
    [InlineData(3, "Holiday Inn", 2)]
    public async Task TestGetAsync(int id, string name, int categoryId)
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            var item = await _repo.GetAsync(id);

            //Assert
            Assert.NotNull(item);
            item.ShouldNotBeNull();

            item.Id.ShouldBe(id);
            item.Name.ShouldBe(name);
            item.CategoryId.ShouldBe(categoryId);
        }
    }

    [Fact]
    public async Task TestGetAsyncBadInvalidId()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            var item = await _repo.GetAsync(-1);

            //Assert
            Assert.Null(item);
            item.ShouldBeNull();
        }
    }

    [Fact]
    public async Task TestGetAsyncBadValidIdNoRecord()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            var item = await _repo.GetAsync(-1);

            //Assert
            Assert.Null(item);
            item.ShouldBeNull();
        }
    }

    //test add good
    [Fact]
    public async Task TestAddAsync()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = 0, CategoryId = 2, ImagePath = "Some Path", Name = "Marriot" };

            //Act
            var result = await _repo.AddOrUpdateAsync(item);

            //Assert
            result.ShouldBeGreaterThan(0);
            item.Id.ShouldBeGreaterThan(0);
            
            //remove the item

            await _repo.DeleteAsync(item.Id);
        }
    }

    //test add bad
    [Fact]
    public async Task TestAddAsyncBadNullItemShouldly()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            Item item = null;

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.AddOrUpdateAsync(item));
        }
    }

    //test update good
    [Fact]
    public async Task TestUpdateGood()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = await _repo.GetAsync(3);

            //Act
            item.Name = "Marriot";
            item.CategoryId = 1;
            item.ImagePath = "This is the way";

            var result = await _repo.AddOrUpdateAsync(item);
            result.ShouldBe(item.Id);

            var itemCheck = await _repo.GetAsync(item.Id);
            itemCheck.Name.ShouldBe("Marriot");
            itemCheck.CategoryId.ShouldBe(1);
            itemCheck.ImagePath.ShouldBe("This is the way");

            //reset the item
            var resetItem = new Item() { Id = 3, CategoryId = 2, ImagePath = "Some Path", Name = "Holiday Inn" };
            await _repo.AddOrUpdateAsync(resetItem);
        }
    }

    //test update bad
    [Fact]
    public async Task TestUpdateBadNoMatchingRecord()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = 70, CategoryId = 2, ImagePath = "Some Path", Name = "Marriot" };

            //Act
            Should.Throw<Exception>(async () => await _repo.AddOrUpdateAsync(item));
        }
    }

    [Fact]
    public async Task TestAddOrUpdateBadInvalidId()
    {         
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = -1, CategoryId = 2, ImagePath = "Some Path", Name = "Marriot" };

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.AddOrUpdateAsync(item));
        }
    }

    //TODO: tests for add/update bad invalid category id
    [Fact]
    public async Task TestAddOrUpdateBadInvalidCategoryId()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = 0, CategoryId = -1, ImagePath = "Some Path", Name = "Marriot" };

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.AddOrUpdateAsync(item));
        }
    }

    //test delete 1 good
    [Fact]
    public async Task TestDeleteAsyncByItemGood()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = 0, CategoryId = 2, ImagePath = "Some Path", Name = "Marriot" };
            var result = await _repo.AddOrUpdateAsync(item);

            //Act
            var deleteResult = await _repo.DeleteAsync(item);

            //Assert
            deleteResult.ShouldBe(item.Id);

            var itemCheck = await _repo.GetAsync(item.Id);
            itemCheck.ShouldBeNull();
        }
    }

    //test delete 1 bad null item
    [Fact]
    public async Task TestDeleteAsyncByItemBadNullItem()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            Item item = null;

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.DeleteAsync(item));
        }
    }

    //test delete 2 good    
    [Fact]
    public async Task TestDeleteAsyncByIdGood()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);
            var item = new Item() { Id = 0, CategoryId = 2, ImagePath = "Some Path", Name = "Marriot" };
            var result = await _repo.AddOrUpdateAsync(item);

            //Act
            var deleteResult = await _repo.DeleteAsync(item.Id);

            //Assert
            deleteResult.ShouldBe(item.Id);

            var itemCheck = await _repo.GetAsync(item.Id);
            itemCheck.ShouldBeNull();
        }
    }

    //test delete 2 bad id < 1
    [Fact]
    public async Task TestDeleteAsyncByIdBadIdLessThan1()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.DeleteAsync(-5));
        }
    }

    //test delete 2 bad > 0 no matching record
    [Fact]
    public async Task TestDeleteAsyncByIdBadNoMatchingRecord()
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.DeleteAsync(100));
        }
    }

    //test exists good
    //test exists bad
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(50, false)]
    public async Task TestExistsAsync(int id, bool expected)
    {
        using (var context = new AnybodysDbContext(_options))
        {
            //Arrange
            _repo = new ItemsRepository(context);

            //Act
            var result = await _repo.ExistsAsync(id);

            //Assert
            result.ShouldBe(expected);
        }
    }
}