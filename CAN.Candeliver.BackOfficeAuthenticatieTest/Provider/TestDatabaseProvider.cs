using CAN.Candeliver.BackOfficeAuthenticatie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CAN.Candeliver.BackOfficeAuthenticatieTest.Services
{
    public static class TestDatabaseProvider
    {
        public static DbContextOptions<ApplicationDbContext> CreateInMemoryDatabaseOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }


        public static DbContextOptions<ApplicationDbContext> CreateMsSQLDatabaseOptions()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
           
            builder.UseSqlServer("Server=.\\SQLEXPRESS; Database=WebshopTest; Trusted_Connection=True;");
            return builder.Options;
        }
    }
}
