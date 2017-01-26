using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAN.Candeliver.BackOfficeAuthenticatie.Models;

namespace CAN.Candeliver.BackOfficeAuthenticatie.Data.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public ApplicationUser FindByUserName(string username)
        {
           return _context.Users.Where(u => u.UserName == username).SingleOrDefault();
        }
    }
}
