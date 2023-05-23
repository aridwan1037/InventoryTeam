using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InventoryManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//code for setting identity and inisialisasi identity
builder.Services.AddDefaultIdentity<User>
(options => options.SignIn.RequireConfirmedAccount = true)
.AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();



var app = builder.Build();
//untuk set awal data role dan akun admin agar nantinya bisa di pisahkan logic webnya
SetRoleOnDatabase.CreateAdminEmployeeRole(app);
SetAdminOnDatabase.CreateAdminDataOnDatabase(app);



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
