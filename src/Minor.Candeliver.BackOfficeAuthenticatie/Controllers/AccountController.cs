using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Minor.Candeliver.BackOfficeAuthenticatie.Models;
using Minor.Candeliver.BackOfficeAuthenticatie.Models.AccountViewModels;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;
using Minor.Candeliver.BackOfficeAuthenticatie.Security;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Minor.Candeliver.BackOfficeAuthenticatie.Services;

namespace Minor.Candeliver.BackOfficeAuthenticatie.Controllers
{
    [Route("api/v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly TokenProviderOptions _options;
        private readonly IAccountService _accountService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IOptions<TokenProviderOptions> options, IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _options = options.Value;
            _accountService = accountService;
        }


        // POST: /Account/Login
        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("BackOfficeLogin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }
            var identity = await _accountService.GetIdentityAsync(model.UserName, model.Password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var user = await _accountService.GetUserAsync(User);
            var response = new LoginResult()
            {
                Access_Token = _accountService.CreateJwtTokenForUser(user),
                Expires_In = (int)_options.Expiration.TotalSeconds

            };
            return Json(response);


        }
        // POST: /Account/Register
        [HttpPost]
        [Route("Register")]
        [SwaggerOperation("BackOfficeRegister")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };


                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = "Role",
                    ClaimValue = "Admin"
                });
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var x = await _userManager.AddClaimAsync(user, new Claim("Role", "Admin"));

                    _logger.LogInformation(3, "User created a new account with password.");
                    return Json("User created a new account with password.");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return Json(ModelState);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [Route("Logoff")]
        [Authorize]
        [SwaggerOperation("BackOfficeLogOff")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return Json("OK");
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }



        #endregion
    }
}
