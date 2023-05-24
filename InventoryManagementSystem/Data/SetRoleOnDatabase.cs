using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Data
{
    public static class SetRoleOnDatabase
    {    //panggil service role manager dari bawaan library identity
         public static async void CreateAdminEmployeeRole(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider
                            .GetRequiredService<RoleManager<IdentityRole>>();

            await SetRolesAsync(roleManager);
        }
        //pembuatan/insert data Role yang sudah ditentukan ke tabel ASPNET ROLE di database
        static async Task SetRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) //cek apakah role sudah ada. jka belum ada maka akan di create role nya
                {
                    await roleManager.CreateAsync(new IdentityRole(role));//proses creation role
                }
            }
        }
    }
}