using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class LostItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LostItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LostItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LostItems.Include(l => l.BorrowedItem).Include(l => l.Item).Include(l => l.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LostItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LostItems == null)
            {
                return NotFound();
            }

            var lostItem = await _context.LostItems
                .Include(l => l.BorrowedItem)
                .Include(l => l.Item)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LostId == id);
            if (lostItem == null)
            {
                return NotFound();
            }

            return View(lostItem);
        }

        // GET: LostItems/Create
        public IActionResult Create()
        {
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId");
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: LostItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LostId,ItemId,UserId,CreateAt,LostDate,NoteItemLost,NoteItemFound,BorrowedId,Status")] LostItem lostItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lostItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", lostItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", lostItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", lostItem.UserId);
            return View(lostItem);
        }

        // GET: LostItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LostItems == null)
            {
                return NotFound();
            }

            var lostItem = _context.LostItems.Include(c=>c.User).Where(d=>d.LostId==id).FirstOrDefault();
            if (lostItem == null)
            {
                return NotFound();
            }
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", lostItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", lostItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", lostItem.UserId);
            return View(lostItem);
        }

        // POST: LostItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LostId,ItemId,UserId,CreateAt,LostDate,NoteItemLost,NoteItemFound,BorrowedId,Status")] LostItem lostItem)
        {
            if (id != lostItem.LostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lostItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LostItemExists(lostItem.LostId))
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
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", lostItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", lostItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", lostItem.UserId);
            return View(lostItem);
        }

        // GET: LostItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LostItems == null)
            {
                return NotFound();
            }

            var lostItem = await _context.LostItems
                .Include(l => l.BorrowedItem)
                .Include(l => l.Item)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LostId == id);
            if (lostItem == null)
            {
                return NotFound();
            }

            return View(lostItem);
        }

        // POST: LostItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LostItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LostItems'  is null.");
            }
            var lostItem = await _context.LostItems.FindAsync(id);
            if (lostItem != null)
            {
                _context.LostItems.Remove(lostItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LostItemExists(int id)
        {
          return (_context.LostItems?.Any(e => e.LostId == id)).GetValueOrDefault();
        }
    }
}
