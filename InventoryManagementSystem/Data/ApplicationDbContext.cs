using System.ComponentModel;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Supplier> Suppliers { get; set;} = default!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base (options) 
    { 

    }
}
