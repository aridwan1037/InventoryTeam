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
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderItemsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var OrderItems = await Search(SearchString);
                return View(OrderItems);
            }

            List<OrderItem> allOrderItems = await GetAllDataFromDatabase();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allOrderItems = allOrderItems.Where(w => w.UserId == userId).ToList();

            }
            return View(allOrderItems);

        }

        private async Task<List<OrderItem>> GetAllDataFromDatabase()
        {
            return await _context.OrderItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .ToListAsync();
            // show all rows in items table
        }

        public async Task<List<OrderItem>> Search(string searchString)
        {
            var orderItem = await _context.OrderItems
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
                orderItem = orderItem.Where(w => w.UserId == userId).ToList();
            }

            return orderItem;
        }
        // public async Task<IActionResult> Index()
        // {
        //     var applicationDbContext = _context.OrderItems.Include(o => o.Item).Include(o => o.User);
        //     return View(await applicationDbContext.ToListAsync());
        // }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.Item)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create(int requestId, RequestItemStatus status)
        {
            var requestItem = _context.RequestItems.Include(c => c.Item).Include(d => d.User).Where(d => d.RequestId == requestId)
            .FirstOrDefault();

            if (requestItem == null)
            {
                return NotFound();
            }


            var orderItem = new OrderItem()
            {
                RequestItem = requestItem,
                RequestId = requestItem.RequestId,
                ItemId = requestItem.ItemId,
                Item = requestItem.Item,
                UserId = requestItem.UserId,
                User = requestItem.User,
                CreateAt = DateTime.Now,
                BorrowDateApproved = requestItem.RequestBorrowDate,
                DueDateApproved = requestItem.RequestDueDate,
            };



            ViewData["statusReqAction"] = status;

            // ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem");
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View(orderItem);
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,RequestId,ItemId,UserId,CreateAt,BorrowDateApproved,DueDateApproved,NoteDonePickUp,NoteWaitingPickUp,Status")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                //kode untuk mencari data request item berdasarkan id yang ditentukan
                var requestItem = _context.RequestItems.Include(c => c.Item).Include(d => d.User)
                .Where(d => d.RequestId == orderItem.RequestId).FirstOrDefault();

                if (requestItem == null)
                {
                    return NotFound();
                }

                _context.Add(orderItem); //untuk save order di database
                await _context.SaveChangesAsync(); //untuk save order di database

                //kode untuk ganti status di req item yang sudah di approved
                requestItem.Status = RequestItemStatus.Approved;//ubah status jadi approved
                requestItem.OrderItemId = orderItem.OrderId;//tambah data orderId di object
                _context.Update(requestItem);//update entitas reqitem ke database
                await _context.SaveChangesAsync();//pasangan dg atasnya. update entitas reqitem ke database

                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", orderItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderItem.UserId);
            return View(orderItem);
        }

        // // GET: OrderItems/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null || _context.OrderItems == null)
        //     {
        //         return NotFound();
        //     }

        //     var orderItem = await _context.OrderItems.FindAsync(id);
        //     if (orderItem == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", orderItem.ItemId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderItem.UserId);
        //     return View(orderItem);
        // }

        // // POST: OrderItems/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("OrderId,RequestId,ItemId,UserId,CreateAt,BorrowDateApproved,RequestDueDateApproved,NoteDonePickUp,NoteWaitingPickUp,Status")] OrderItem orderItem)
        // {
        //     if (id != orderItem.OrderId)
        //     {
        //         return NotFound();
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(orderItem);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!OrderItemExists(orderItem.OrderId))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", orderItem.ItemId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderItem.UserId);
        //     return View(orderItem);
        // }

        // // GET: OrderItems/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null || _context.OrderItems == null)
        //     {
        //         return NotFound();
        //     }

        //     var orderItem = await _context.OrderItems
        //         .Include(o => o.Item)
        //         .Include(o => o.User)
        //         .FirstOrDefaultAsync(m => m.OrderId == id);
        //     if (orderItem == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(orderItem);
        // }

        // // POST: OrderItems/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     if (_context.OrderItems == null)
        //     {
        //         return Problem("Entity set 'ApplicationDbContext.OrderItems'  is null.");
        //     }
        //     var orderItem = await _context.OrderItems.FindAsync(id);
        //     if (orderItem != null)
        //     {
        //         _context.OrderItems.Remove(orderItem);
        //     }

        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool OrderItemExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToCsv(string searchString)
        {
            var orderItems = _context.OrderItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                orderItems = orderItems
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
                    csvWriter.WriteRecords(orderItems);
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