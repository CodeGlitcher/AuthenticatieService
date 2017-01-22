using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minor.Candeliver.BackOfficeAuthenticatie.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        
        [HttpGet]
        [Route("public")]
        public IActionResult PublicTest()
        {
            return Json("Hello everyone");
        }


        [HttpGet]
        [Route("Private")]
        [Authorize(Roles = "Admin")]
        public IActionResult Private()
        {
            return Json("Hello me");
        }
    }
}
