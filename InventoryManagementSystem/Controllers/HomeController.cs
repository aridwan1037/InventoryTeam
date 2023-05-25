using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InventoryManagementSystem.Data;

namespace InventoryManagementSystem.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
    {
        _logger = logger;
        _userManager=userManager;
        _context=context;
    }

    public IActionResult Index()
    {
        var userId = _userManager.GetUserId(User);
        var totalRequestsItems = _context.RequestItems.Where(c => c.UserId == userId).Count();
        var totalOrderItems = _context.OrderItems.Where(c => c.UserId == userId).Count();
        var totalBorrwedItems = _context.BorrowedItems.Where(c => c.UserId == userId).Count();
        var totalGoodReceipt = _context.GoodReceipts.Where(c => c.UserId == userId).Count();

        if (User.IsInRole("Admin"))
        {
            totalRequestsItems = _context.RequestItems.Count();
            totalOrderItems = _context.OrderItems.Count();
            totalBorrwedItems = _context.BorrowedItems.Count();
            totalGoodReceipt = _context.GoodReceipts.Count();
        }

        ViewBag.TotalRequestsBorrow = totalRequestsItems;
        ViewBag.TotalOrderItems = totalOrderItems;
        ViewBag.TotalBorrwedItems = totalBorrwedItems;
        ViewBag.TotalGoodReceipt = totalGoodReceipt;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
