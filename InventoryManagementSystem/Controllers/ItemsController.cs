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
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: Items
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Items.Include(i => i.Category).Include(i => i.SubCategory).Include(i => i.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.SubCategory)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdItem,KodeItem,Name,Description,Availability,CategoryId,SubCategoryId,SupplierId")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.KodeItem = GenerateItemCode(item.CategoryId, item.SubCategoryId);
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", item.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", item.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", item.SupplierId);
            return View(item);
        }
        private string GenerateItemCode(int categoryId, int subCategoryId)
        {
            var categoryCode = _context.Categories.FirstOrDefault(c => c.IdCategory == categoryId)?.CategoryCode;
            var subCategoryCode = _context.SubCategories.FirstOrDefault(s => s.IdSubCategory == subCategoryId)?.SubCategoryCode;

            var itemCode = $"{categoryCode}-{subCategoryCode}-1";
            var itemsCount = 1;

            // Check if the generated item code already exists
            while (_context.Items.Any(i => i.KodeItem == itemCode))
            {
                itemsCount++;
                itemCode = $"{categoryCode}-{subCategoryCode}-{itemsCount}";
            }

            return itemCode;
        }
        public IActionResult GetSubcategoriesByCategory(int categoryId)
        {
            var subcategories = _context.SubCategories.Where(s => s.CategoryId == categoryId).Select(s => new { value = s.IdSubCategory, text = s.SubCategoryName }).ToList();
            return Json(subcategories);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", item.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", item.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", item.SupplierId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdItem,KodeItem,Name,Description,Availability,CategoryId,SubCategoryId,SupplierId")] Item item)
        {
            if (id != item.IdItem)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.IdItem))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", item.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", item.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", item.SupplierId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.SubCategory)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.IdItem == id);
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
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.IdItem == id)).GetValueOrDefault();
        }
    }
}
