using System.ComponentModel;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<SubCategory> SubCategories { get; set; } = default!;
    public DbSet<Supplier> Suppliers { get; set;} = default!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base (options) 
    { 

    }
    protected override void OnModelCreating(ModelBuilder builder)
    { //membuat fluentApi rule database User
    base.OnModelCreating(builder);
    builder.Entity<User>()
    .Property(e => e.FirstName)
    .HasMaxLength(200);

    builder.Entity<User>()
    .Property(e => e.LastName)
    .HasMaxLength(200);

     builder.Entity<User>()
    .Property(e => e.IdEmployee)
    .HasMaxLength(10);
    }
}
