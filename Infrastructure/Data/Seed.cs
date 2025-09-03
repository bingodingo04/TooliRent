using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class Seed
    {
        public static async Task RunAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<TooliRentDbContext>();
            await ctx.Database.MigrateAsync();

            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            foreach (var r in new[] { "Admin", "Member" })
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole<Guid>(r));

            var admin = await userMgr.FindByEmailAsync("admin@tooli.local");
            if (admin is null)
            {
                admin = new AppUser { Id = Guid.NewGuid(), Email = "admin@tooli.local", UserName = "admin@tooli.local", EmailConfirmed = true };
                await userMgr.CreateAsync(admin, "Admin123!");
                await userMgr.AddToRolesAsync(admin, new[] { "Admin" });
            }

            var member = await userMgr.FindByEmailAsync("member@tooli.local");
            if (member is null)
            {
                member = new AppUser { Id = Guid.NewGuid(), Email = "member@tooli.local", UserName = "member@tooli.local", EmailConfirmed = true };
                await userMgr.CreateAsync(member, "Member123!");
                await userMgr.AddToRolesAsync(member, new[] { "Member" });
            }

            if (!await ctx.Categories.AnyAsync())
            {
                var cat1 = new Category { Id = Guid.NewGuid(), Name = "Handverktyg" };
                var cat2 = new Category { Id = Guid.NewGuid(), Name = "Elverktyg" };
                ctx.Categories.AddRange(cat1, cat2);

                ctx.Tools.AddRange(
                    new Tool { Id = Guid.NewGuid(), Name = "Hammare", Category = cat1 },
                    new Tool { Id = Guid.NewGuid(), Name = "Skruvdragare", Category = cat2, SerialNumber = "SD-1001" },
                    new Tool { Id = Guid.NewGuid(), Name = "Sticksåg", Category = cat2, SerialNumber = "JS-8842" }
                );
                await ctx.SaveChangesAsync();
            }
        }
    }
}
