using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;
using System.Globalization;

namespace InventoryManagementSystem.Controllers
{
    public class RequestItemsController : Controller
    {

        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public RequestItemsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Employee")]

        // GET: RequestItems
        public async Task<IActionResult> Index(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var RequestItems = await Search(SearchString);
                return View(RequestItems);
            }

            List<RequestItem> allRequestItems = await GetAllDataFromDatabase();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allRequestItems = allRequestItems.Where(w => w.UserId == userId).ToList();

            }
            return View(allRequestItems);

        }

        private async Task<List<RequestItem>> GetAllDataFromDatabase()
        {
            return await _context.RequestItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .ToListAsync();
            // show all rows in items table
        }

        public async Task<List<RequestItem>> Search(string searchString)
        {
            var requestItem = await _context.RequestItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .Where(
                s => s.Item!.Name!.ToLower().Contains(searchString.ToLower()) ||
                s.Item!.KodeItem!.ToLower().Contains(searchString.ToLower()) ||
                s.User!.UserName!.ToLower().Contains(searchString.ToLower()) ||
                s.User!.Email!.ToLower().Contains(searchString.ToLower())
            ).ToListAsync();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                requestItem = requestItem.Where(w => w.UserId == userId).ToList();
            }

            return requestItem;
        }

        // GET: RequestItems/Details/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RequestItems == null)
            {
                return NotFound();
            }

            var requestItem = await _context.RequestItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (requestItem == null)
            {
                return NotFound();
            }

            return View(requestItem);
        }

        // GET: RequestItems/Create
        [Authorize(Roles = "Employee")]
        public IActionResult Create(int? itemId)
        {
            // ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem");
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            // return View();

            if (User.IsInRole("Admin"))
            {
                ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem");
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
                return View();
            }

            else
            {
                if (itemId == null) return NotFound();
                ViewData["ItemId"] = new SelectList(_context.Items
                .Where(c => c.IdItem == itemId), "IdItem", "KodeItem");

                var item = _context.Items.Where(c => c.IdItem == itemId).FirstOrDefault();
                //  ViewData["UserId"] = _userManager.GetUserId(User);

                var requestItem = new RequestItem
                {
                    Item = item,
                    ItemId = (int)itemId!,
                    UserId = _userManager.GetUserId(User)!,
                    RequestBorrowDate = DateTime.Now,
                    RequestDueDate = DateTime.Now.AddDays(10), // Autofill with current date/time default 10 days
                };

                return View(requestItem);

            }
        }

        // POST: RequestItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,ItemId,UserId,CreateAt,RequestBorrowDate,RequestDueDate,NoteRequest,Status")] RequestItem requestItem)
        {
            // var checkItemChoose=_context.RequestItems.Any(c=>c.ItemId==requestItem.ItemId);
            // if(checkItemChoose)
            // {
            //     return BadRequest();
            // }

            if (ModelState.IsValid)
            {
                var item = _context.Items.Where(c => c.IdItem == requestItem.ItemId).FirstOrDefault();
                if (item == null)
                {
                    return NotFound();
                }
                item.Availability = false;
                _context.Update(item);
                _context.Add(requestItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", requestItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItem.UserId);
            return View(requestItem);
        }

        // GET: RequestItems/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RequestItems == null)
            {
                return NotFound();
            }

            var requestItem = await _context.RequestItems.Include(c => c.Item).FirstOrDefaultAsync(a => a.RequestId == id);
            if (requestItem == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", requestItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", requestItem.UserId);
            return View(requestItem);

        }

        // GET: RequestItems/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null || _context.RequestItems == null)
            {
                return NotFound();
            }

            var requestItem = await _context.RequestItems.Include(c => c.Item).FirstOrDefaultAsync(a => a.RequestId == id);
            if (requestItem == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", requestItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", requestItem.UserId);
            return View(requestItem);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id, [Bind("RequestId,ItemId,UserId,CreateAt,RequestBorrowDate,RequestDueDate,NoteRequest,NoteActionRequest,Status")] RequestItem requestItem)
        {
            var item = _context.Items.Where(c => c.IdItem == requestItem.ItemId).FirstOrDefault();
            if (id != requestItem.RequestId || item == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { //di dalam try karena default code generator ada di sini. baiknya, jika database tb tb disconnect, maka bisa nangkep error
                    item.Availability = true; //untuk ubah status avalability pada otem jadi true
                    _context.Update(item);
                    await _context.SaveChangesAsync();


                    requestItem.Status = RequestItemStatus.Rejected;
                    _context.Update(requestItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestItemExists(requestItem.RequestId))
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
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", requestItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItem.UserId);
            return View(requestItem);
        }

        // POST: RequestItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,ItemId,UserId,CreateAt,RequestBorrowDate,RequestDueDate,NoteRequest,NoteActionRequest,Status")] RequestItem requestItem)
        {
            if (id != requestItem.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestItemExists(requestItem.RequestId))
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
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", requestItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItem.UserId);
            return View(requestItem);
        }

        // GET: RequestItems/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RequestItems == null)
            {
                return NotFound();
            }

            var requestItem = await _context.RequestItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (requestItem == null)
            {
                return NotFound();
            }

            return View(requestItem);
        }

        // POST: RequestItems/Delete/5
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RequestItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RequestItems'  is null.");
            }
            var requestItem = await _context.RequestItems.FindAsync(id);
            if (requestItem != null)
            {
                _context.RequestItems.Remove(requestItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (_context.RequestItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RequestItems'  is null.");
            }
            var requestItem = await _context.RequestItems.FindAsync(id);


            if (requestItem == null)
            {
                return NotFound();
            }
            requestItem.Status = RequestItemStatus.Cancel;
            try
            {
                _context.Update(requestItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RequestItemExists(int id)
        {
            return (_context.RequestItems?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToCsv(string searchString)
        {
            var requestItems = _context.RequestItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                requestItems = requestItems
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
                    csvWriter.WriteRecords(requestItems);
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

