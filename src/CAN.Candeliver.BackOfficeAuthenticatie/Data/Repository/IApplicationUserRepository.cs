using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using System;

namespace CAN.Candeliver.BackOfficeAuthenticatie.Data.Repository
{
    public interface IApplicationUserRepository : IDisposable
    {
        ApplicationUser FindByUserName(string username);
    }
}