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
    public DbSet<Supplier> Suppliers { get; set; } = default!;
    public DbSet<RequestItem> RequestItems { get; set; } = default!;
    // public DbSet<BorrowedItem> BorrowedItems { get; set; } = default!;
    // public DbSet<OrderItem> OrderItems { get; set; } = default!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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

        // builder.Entity<BorrowedItem>()
        // .HasOne(b => b.User)
        // .WithMany()
        // .HasForeignKey(b => b.UserId);

        builder.Entity<RequestItem>()
        .HasOne(b => b.User)
        .WithMany()
        .HasForeignKey(b => b.UserId);
        builder.Entity<OrderItem>()
        .HasOne(b => b.User)
        .WithMany()
        .HasForeignKey(b => b.UserId);
        //====================================================
        builder.Entity<RequestItem>()
        .HasOne(r => r.OrderItem)
        .WithOne(o => o.RequestItem!)
        .HasForeignKey<OrderItem>(o => o.RequestId);

        // builder.Entity<OrderItem>()
        // .HasOne(f => f.BorrowedItem)
        // .WithOne(o => o!.OrderItem)
        // .HasForeignKey<BorrowedItem>(o => o.OrderId);
    }
}
