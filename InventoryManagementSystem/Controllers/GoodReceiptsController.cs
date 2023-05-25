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
    [Authorize]
    public class GoodReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoodReceiptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GoodReceipts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GoodReceipts.Include(g => g.BorrowedItem).Include(g => g.Item).Include(g => g.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GoodReceipts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GoodReceipts == null)
            {
                // return NotFound();
            }

            var goodReceipt = await _context.GoodReceipts
                .Include(g => g.BorrowedItem)
                .Include(g => g.Item)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.ReceiptId == id);
            if (goodReceipt == null)
            {
                return NotFound();
            }

            return View(goodReceipt);
        }

        // GET: GoodReceipts/Create
        public IActionResult Create(int? borrowedId)
        {
            var borrowedItem = _context.BorrowedItems.Include(c => c.Item).Include(d => d.User)
            .Where(d => d.BorrowedId == borrowedId).FirstOrDefault();

            if (borrowedItem == null)
            {
                return NotFound();
            }

            var goodReceipt = new GoodReceipt()
            {
                BorrowedItem = borrowedItem,
                BorrowedId = borrowedItem.OrderId,
                ItemId = borrowedItem.ItemId,
                Item = borrowedItem.Item,
                UserId = borrowedItem.UserId,
                User = borrowedItem.User,
                CreateAt = DateTime.Now,
                ReceivedDate = DateTime.Now,
            };

            return View(goodReceipt);
        }

        // POST: GoodReceipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceiptId,ItemId,UserId,CreateAt,ReceivedDate,MissedDueDate,NoteItemReturned,NoteItemLost,NoteItemBroken,BorrowedId,Status")] GoodReceipt goodReceipt)
        {

            if (ModelState.IsValid && goodReceipt.Status != GoodReceiptStatus.Lost)
            {
                var borrowedItem = _context.BorrowedItems.Where(d => d.BorrowedId == goodReceipt.BorrowedId).FirstOrDefault();
                var item = _context.Items.Where(c => c.IdItem == goodReceipt.ItemId).FirstOrDefault();

                if (borrowedItem == null || item == null)
                {
                    return NotFound();
                }

                _context.Add(goodReceipt);
                await _context.SaveChangesAsync();

                borrowedItem.Status = BorrowedItemStatus.DoneBorrowing;
                borrowedItem.ReceiptId = goodReceipt.ReceiptId;

                _context.Update(borrowedItem);
                await _context.SaveChangesAsync();

                item.Availability = goodReceipt.Status == GoodReceiptStatus.Returned;
                _context.Update(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(goodReceipt);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lost([Bind("ReceiptId,ItemId,UserId,CreateAt,ReceivedDate,MissedDueDate,NoteItemReturned,NoteItemLost,NoteItemBroken,BorrowedId,Status")] GoodReceipt goodReceipt)
        {

            if (ModelState.IsValid && goodReceipt.Status == GoodReceiptStatus.Lost)
            {
                var borrowedItem = _context.BorrowedItems.Where(d => d.BorrowedId == goodReceipt.BorrowedId).FirstOrDefault();
                var item = _context.Items.Where(c => c.IdItem == goodReceipt.ItemId).FirstOrDefault();

                if (borrowedItem == null || item == null)
                {
                    return NotFound();
                }

                var lostItem = new LostItem()
                {
                    BorrowedItem = borrowedItem,
                    BorrowedId = borrowedItem.OrderId,
                    ItemId = borrowedItem.ItemId,
                    Item = borrowedItem.Item,
                    UserId = borrowedItem.UserId,
                    User = borrowedItem.User,
                    NoteItemLost = goodReceipt.NoteItemLost,
                    CreateAt = DateTime.Now,
                    LostDate = DateTime.Now,
                    Status = LostItemStatus.Active
                };

                _context.Add(lostItem);
                await _context.SaveChangesAsync();

                borrowedItem.Status = BorrowedItemStatus.DoneAndLost;
                borrowedItem.LostId = lostItem.LostId;

                _context.Update(borrowedItem);
                await _context.SaveChangesAsync();

                item.Availability = false;
                _context.Update(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), "LostItems");
            }

            return View(goodReceipt);
        }


        // GET: GoodReceipts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GoodReceipts == null)
            {
                return NotFound();
            }

            var goodReceipt = await _context.GoodReceipts.FindAsync(id);
            if (goodReceipt == null)
            {
                return NotFound();
            }
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", goodReceipt.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", goodReceipt.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", goodReceipt.UserId);
            return View(goodReceipt);
        }

        // POST: GoodReceipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceiptId,ItemId,UserId,CreateAt,ReceivedDate,MissedDueDate,NoteItemReturned,NoteItemLost,NoteItemBroken,BorrowedId,Status")] GoodReceipt goodReceipt)
        {
            if (id != goodReceipt.ReceiptId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goodReceipt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodReceiptExists(goodReceipt.ReceiptId))
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
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", goodReceipt.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", goodReceipt.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", goodReceipt.UserId);
            return View(goodReceipt);
        }

        // GET: GoodReceipts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GoodReceipts == null)
            {
                return NotFound();
            }

            var goodReceipt = await _context.GoodReceipts
                .Include(g => g.BorrowedItem)
                .Include(g => g.Item)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.ReceiptId == id);
            if (goodReceipt == null)
            {
                return NotFound();
            }

            return View(goodReceipt);
        }

        // POST: GoodReceipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GoodReceipts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GoodReceipts'  is null.");
            }
            var goodReceipt = await _context.GoodReceipts.FindAsync(id);
            if (goodReceipt != null)
            {
                _context.GoodReceipts.Remove(goodReceipt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodReceiptExists(int id)
        {
            return (_context.GoodReceipts?.Any(e => e.ReceiptId == id)).GetValueOrDefault();
        }
    }
}