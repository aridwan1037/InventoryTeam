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
    public class BorrowedItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvirontment;
        private readonly UserManager<User> _userManager;

        public BorrowedItemsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _webHostEnvirontment = hostEnvironment;
            _userManager = userManager;
        }
        [Authorize]
        // GET: BorrowedItems
        public async Task<IActionResult> Index(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var BorrowedItems = await Search(SearchString);
                return View(BorrowedItems);
            }

            List<BorrowedItem> allBorrowedItems = await GetAllDataFromDatabase();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allBorrowedItems = allBorrowedItems.Where(w => w.UserId == userId).ToList();

            }
            return View(allBorrowedItems);

        }

        private async Task<List<BorrowedItem>> GetAllDataFromDatabase()
        {
            return await _context.BorrowedItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .ToListAsync();
            // show all rows in items table
        }

        public async Task<List<BorrowedItem>> Search(string searchString)
        {
            var borrowedItem = await _context.BorrowedItems
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
                borrowedItem = borrowedItem.Where(w => w.UserId == userId).ToList();
            }

            return borrowedItem;
        }

        // GET: BorrowedItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BorrowedItems == null)
            {
                return NotFound();
            }

            var borrowedItem = await _context.BorrowedItems
                .Include(b => b.Item)
                .Include(b => b.OrderItem)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BorrowedId == id);
            if (borrowedItem == null)
            {
                return NotFound();
            }

            return View(borrowedItem);
        }

        // GET: BorrowedItems/Create
        public IActionResult Create(int orderId)
        {
            var orderItem = _context.OrderItems.Include(c => c.Item).Include(d => d.User).Where(d => d.OrderId == orderId)
           .FirstOrDefault();

            if (orderItem == null)
            {
                return NotFound();
            }

            var borrowedItem = new BorrowedItemViewModel()
            {
                OrderItem = orderItem,
                OrderId = orderItem.OrderId,
                ItemId = orderItem.ItemId,
                Item = orderItem.Item,
                UserId = orderItem.UserId,
                User = orderItem.User,
                CreateAt = DateTime.Now,
                BorrowedDate = orderItem.BorrowDateApproved,
                DueDate = orderItem.DueDateApproved,
                Status = BorrowedItemStatus.StillBorrowed
            };

            return View(borrowedItem);
        }

        // POST: BorrowedItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BorrowedItemViewModel borrowedItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = await UploadFile(borrowedItemViewModel);
                var orderItem = _context.OrderItems.Include(c => c.Item).Include(d => d.User).Where(d => d.OrderId == borrowedItemViewModel.OrderId)
                .FirstOrDefault();

                if (orderItem == null)
                {
                    return NotFound();
                }

                var borrowedItemAction = new BorrowedItem //untuk simpan ke database
                {
                    OrderId = orderItem.OrderId,
                    ItemId = borrowedItemViewModel.ItemId,
                    PicturePath = uniqueFileName,
                    UserId = borrowedItemViewModel.UserId,
                    CreateAt = DateTime.Now,
                    BorrowedDate = borrowedItemViewModel.BorrowedDate,
                    DueDate = borrowedItemViewModel.DueDate,
                    NoteBorrowed = borrowedItemViewModel.NoteBorrowed,

                };

                _context.Add(borrowedItemAction); //simpan data borrowed lebih dulu
                await _context.SaveChangesAsync(); //simpan data borrowed lebih dulu

                orderItem.Status = OrderItemStatus.DonePickUp; //lalu pindahke tabel order item untuk ganti statusnya jadi done pick up
                orderItem.BorrowedId = borrowedItemViewModel.BorrowedId; //lalu tabel order item di tambah id borrowed
                _context.Update(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", borrowedItemViewModel.ItemId);
            ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", borrowedItemViewModel.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowedItemViewModel.UserId);
            return View(borrowedItemViewModel);
        }

        private async Task<String?> UploadFile(BorrowedItemViewModel borrowedItemViewModel)
        {
            //process the uploaded file
            //example : save file to a directory
            if (borrowedItemViewModel.Picture != null && borrowedItemViewModel.Picture.Length > 0)
            {
                string fileName = GetUniqueFileName(borrowedItemViewModel.Picture.FileName);

                string filePath = Path.Combine(_webHostEnvirontment.WebRootPath, "uploads", fileName);
                //webhostenvirontment itu get alamat di wwroot untuk set alaamt image yg di upload agar di save ke wwroot
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await borrowedItemViewModel.Picture.CopyToAsync(fileStream);
                }
                return fileName;
            }
            return null;
        }

        private string GetUniqueFileName(string fileName)
        {
            //Generate a unique file name using a combination of timestamp and original file name
            string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) +
            "_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(fileName);

            return uniqueFileName;
        }

        // GET: BorrowedItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BorrowedItems == null)
            {
                return NotFound();
            }

            var borrowedItem = await _context.BorrowedItems.FindAsync(id);
            if (borrowedItem == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", borrowedItem.ItemId);
            ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", borrowedItem.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowedItem.UserId);
            return View(borrowedItem);
        }

        // POST: BorrowedItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("BorrowedId,OrderId,ReceiptId,ItemId,UserId,CreateAt,BorrowedDate,DueDate,NoteBorrowed,PicturePath,Status")] BorrowedItem borrowedItem)
        {
            if (id != borrowedItem.BorrowedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowedItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowedItemExists(borrowedItem.BorrowedId))
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
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", borrowedItem.ItemId);
            ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", borrowedItem.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowedItem.UserId);
            return View(borrowedItem);
        }

        // GET: BorrowedItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BorrowedItems == null)
            {
                return NotFound();
            }

            var borrowedItem = await _context.BorrowedItems
                .Include(b => b.Item)
                .Include(b => b.OrderItem)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BorrowedId == id);
            if (borrowedItem == null)
            {
                return NotFound();
            }

            return View(borrowedItem);
        }

        // POST: BorrowedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BorrowedItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BorrowedItems'  is null.");
            }
            var borrowedItem = await _context.BorrowedItems.FindAsync(id);
            if (borrowedItem != null)
            {
                _context.BorrowedItems.Remove(borrowedItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowedItemExists(int id)
        {
            return (_context.BorrowedItems?.Any(e => e.BorrowedId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToCsv(string searchString)
        {
            var borrowedItems = _context.BorrowedItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                borrowedItems = borrowedItems
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
                    csvWriter.WriteRecords(borrowedItems);
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
