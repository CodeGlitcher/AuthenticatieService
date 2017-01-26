using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAN.Candeliver.BackOfficeAuthenticatie.Services
{
    public interface IAccountService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> CreateJwtTokenForUserAsync(ApplicationUser user);
        
        Task<ApplicationUser> RegisterAsync(string username, string password, string role);
        Task<ClaimsIdentity> GetIdentityAsync(string username, string password);
        Task<bool> AddRoleAsync(ApplicationUser user, string role);
        ApplicationUser GetUserAsync(string username);
    }
}
