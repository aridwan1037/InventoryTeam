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
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderItems.Include(o => o.Item).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

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
    }
}