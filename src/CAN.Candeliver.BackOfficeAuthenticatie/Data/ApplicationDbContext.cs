using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using CAN.Candeliver.BackOfficeAuthenticatie.Data.Repository;
using CAN.Candeliver.BackOfficeAuthenticatie.Services;

namespace CAN.Candeliver.BackOfficeAuthenticatie.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Constructor. Ensures the createing of the database
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
                      
        }

        internal static async void SeedDb(IApplicationBuilder app)
        {
            using (var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>())
            using (var repo = app.ApplicationServices.GetRequiredService<IApplicationUserRepository>())
            using (var accountService = app.ApplicationServices.GetRequiredService<IAccountService>())
            {
                await context.Database.EnsureCreatedAsync();
                if (repo.FindByUserName("Kees") == null)
                {

                  await accountService.RegisterAsync("Kees", "DeKoning", "Sales");
                }

                if (repo.FindByUserName("Dennis") == null)
                {
                  await  accountService.RegisterAsync("Dennis", "Inpakker", "Magazijn");
                }
            }
        }
    }
}
