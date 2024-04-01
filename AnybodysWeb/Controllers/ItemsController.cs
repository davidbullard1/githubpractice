using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnybodysWeb.Data;
using AnybodysWebModels;
using AnybodysWebDBLibrary;
using AnybodysWebServiceLayer;

namespace AnybodysWeb.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IItemsService _itemsService;

        private readonly SelectList _categories;

        public ItemsController(ICategoriesService categoriesService, IItemsService itemsService)
        {
            _categoriesService = categoriesService;
            _itemsService = itemsService;
            _categories = new SelectList(_categoriesService.GetAllAsync().Result, "Id", "Name");
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _itemsService.GetAllAsync());
        }

        public async Task<IActionResult> Top10Items()
        {
            var items = await _itemsService.GetAllAsync();
            return View(items.OrderBy(x => x.Name).Take(10));
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _itemsService.GetAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = _categories;
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImagePath,CategoryId")] Item item)
        {
            if (item.CategoryId is null)
            {
                ModelState.AddModelError("CategoryId", "Category is required");
                ViewData["Categories"] = _categories;
                return View(item);
            }
            if (ModelState.IsValid)
            {
                await _itemsService.AddOrUpdateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = _categories;
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _itemsService.GetAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = _categories;
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,CategoryId")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (item.CategoryId is null)
            {
                ModelState.AddModelError("CategoryId", "Category is required");
                ViewData["Categories"] = _categories;
                return View(item);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _itemsService.AddOrUpdateAsync(item);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = _categories;
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _itemsService.GetAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _itemsService.GetAsync(id);
            if (item != null)
            {
                await _itemsService.DeleteAsync(item);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _itemsService.ExistsAsync(id).Result;
        }
    }
}
