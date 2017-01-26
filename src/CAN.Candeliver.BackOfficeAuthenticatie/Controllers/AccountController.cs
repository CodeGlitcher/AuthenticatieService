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
using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using CAN.Candeliver.BackOfficeAuthenticatie.Models.AccountViewModels;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;
using CAN.Candeliver.BackOfficeAuthenticatie.Security;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CAN.Candeliver.BackOfficeAuthenticatie.Services;

namespace CAN.Candeliver.BackOfficeAuthenticatie.Controllers
{
    [Route("api/v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly TokenProviderOptions _options;
        private readonly IAccountService _accountService;

        public AccountController(
            ILogger<AccountController> logger,
            IOptions<TokenProviderOptions> options, IAccountService accountService)
        {
            _logger = logger;
            _options = options.Value;
            _accountService = accountService;
        }



        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("BackOfficeLogin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResult() { ErrorMessage = "Onvolledige input" });
            }
            try
            {
                var identity = await _accountService.GetIdentityAsync(model.UserName, model.Password);
                if (identity == null)
                {
                    _logger.LogInformation($"Login user failed");
                    return BadRequest(new ErrorResult() { ErrorMessage = "Verkeerde combinatie gebruikersnaam en wachtwoord" });
                }

                var user = _accountService.GetUserAsync(model.UserName);
                var response = new LoginResult()
                {
                    Access_Token = await _accountService.CreateJwtTokenForUserAsync(user),
                    Expires_In = (int)_options.Expiration.TotalSeconds

                };
                _logger.LogInformation($"Login user succces");
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Login user failed: {e.Message}");
                _logger.LogDebug($"Login user failed: {e.StackTrace}");
                return BadRequest(new ErrorResult() { ErrorMessage = "Login service niet bereikbaar" });
            }


        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: /Account/Register
        [HttpPost]
        [Route("Register")]
        [SwaggerOperation("BackOfficeRegister")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            try
            {
                var result = await _accountService.RegisterAsync(model.UserName, model.Password, model.Role);
                if (result == null)
                {
                    return BadRequest("User registration failed");
                }
                return Json("User created a new account with password.");

            }
            catch (Exception e)
            {
                _logger.LogError($"Creating user failed: {e.Message}");
                _logger.LogDebug($"Creating user failed: {e.StackTrace}");
                return BadRequest("User registration exception");
            }
        }

    }
}
