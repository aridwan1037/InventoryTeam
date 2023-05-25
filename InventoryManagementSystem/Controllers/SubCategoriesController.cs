using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubCategories
        public async Task<IActionResult> Index(string SearchString)
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                var SubCategories = await Search(SearchString);
                return View(SubCategories);
            }

            var allSubCategories = await _context.SubCategories
                .Include(i => i.Category)
                .ToListAsync();
            return View(allSubCategories);
        }
        public async Task<List<SubCategory>> Search(string searchString)
        {
            var subCategories = await _context.SubCategories
                .Include(i => i.Category)
                .Where(s => s.SubCategoryName != null && s.SubCategoryName.ToLower().Contains(searchString.ToLower()) ||
                s.Description!.ToLower().Contains(searchString.ToLower())
                || s.Category.CategoryName!.ToLower().Contains(searchString.ToLower()) ||
                s.SubCategoryCode!.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();
            return subCategories;
        }
        // public async Task<IActionResult> Index()
        // {
        //     var applicationDbContext = _context.SubCategories.Include(s => s.Category);
        //     return View(await applicationDbContext.ToListAsync());
        // }

        // GET: SubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubCategories == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.IdSubCategory == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // GET: SubCategories/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName");
            return View();
        }

        // POST: SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSubCategory,SubCategoryCode,SubCategoryName,CategoryId,Description")] SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // GET: SubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubCategories == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSubCategory,SubCategoryCode,SubCategoryName,CategoryId,Description")] SubCategory subCategory)
        {
            if (id != subCategory.IdSubCategory)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCategoryExists(subCategory.IdSubCategory))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // GET: SubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubCategories == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.IdSubCategory == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubCategories'  is null.");
            }
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory != null)
            {
                _context.SubCategories.Remove(subCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCategoryExists(int id)
        {
            return (_context.SubCategories?.Any(e => e.IdSubCategory == id)).GetValueOrDefault();
        }
    }
}
