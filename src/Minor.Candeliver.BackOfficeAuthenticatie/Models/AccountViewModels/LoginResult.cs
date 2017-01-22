using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minor.Candeliver.BackOfficeAuthenticatie.Models.AccountViewModels
{
    public class LoginResult
    {
        public string Access_Token { get; internal set; }
        public int Expires_In { get; internal set; }
    }
}
