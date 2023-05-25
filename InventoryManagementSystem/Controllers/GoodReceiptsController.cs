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
using Microsoft.AspNetCore.Identity;
using CsvHelper;
using System.Globalization;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class GoodReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public GoodReceiptsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GoodReceipts
        public async Task<IActionResult> Index(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var goodReceipts = await Search(SearchString);
                return View(goodReceipts);
            }

            List<GoodReceipt> allGoodReceipts = await GetAllDataFromDatabase();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allGoodReceipts = allGoodReceipts.Where(w => w.UserId == userId).ToList();

            }
            return View(allGoodReceipts);

        }

        private async Task<List<GoodReceipt>> GetAllDataFromDatabase()
        {
            return await _context.GoodReceipts
            .Include(c => c.Item)
            .Include(c => c.User)
            .Include(g => g.BorrowedItem)
            .ToListAsync();
            // show all rows in items table
        }

        public async Task<List<GoodReceipt>> Search(string searchString)
        {
            var goodReceipts = await _context.GoodReceipts
            .Include(c => c.Item)
            .Include(c => c.User)
            .Include(g => g.BorrowedItem)
            .Where(
                s => s.Item!.Name!.ToLower().Contains(searchString.ToLower()) ||
                s.Item!.KodeItem!.ToLower().Contains(searchString.ToLower()) ||
                s.User!.UserName!.ToLower().Contains(searchString.ToLower()) ||
                s.User!.Email!.ToLower().Contains(searchString.ToLower())
            ).ToListAsync();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                goodReceipts = goodReceipts.Where(w => w.UserId == userId).ToList();
            }

            return goodReceipts;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToCsv(string searchString)
        {
            var goodReceipts = _context.GoodReceipts
                .Include(r => r.Item)
                .Include(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                goodReceipts = goodReceipts
                    .Where(r => r.Item != null && r.Item.Name.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            // Membuat StringWriter untuk menulis data CSV
            using (var sw = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    // Menulis header kolom
                    csvWriter.WriteHeader<RequestItem>();

                    csvWriter.NextRecord();

                    // Menulis data baris
                    csvWriter.WriteRecords(goodReceipts);
                }

                // Mengatur header respons HTTP untuk file CSV
                Response.Headers.Add("Content-Disposition", "attachment; filename=request_items.csv");
                Response.ContentType = "text/csv";

                // Menulis data CSV ke respons HTTP
                return Content(sw.ToString());
            }
        }
    
    }
}