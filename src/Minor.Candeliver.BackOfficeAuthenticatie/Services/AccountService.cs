using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minor.Candeliver.BackOfficeAuthenticatie.Controllers;
using Minor.Candeliver.BackOfficeAuthenticatie.Models;
using Minor.Candeliver.BackOfficeAuthenticatie.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Minor.Candeliver.BackOfficeAuthenticatie.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountController> _logger;
        private readonly TokenProviderOptions _options;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IOptions<TokenProviderOptions> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _options = options.Value;
        }

        /// <summary>
        /// Create a jwt token for a given user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string CreateJwtTokenForUser(ApplicationUser user)
        {
            var now = DateTime.UtcNow;
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub  (subject / user) claims.
            // You can add other claims here, if you want:
            var claims = new List<Claim>()
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.Ticks.ToString(),
                    ClaimValueTypes.Integer64),
            };


            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(_options.Expiration),
            signingCredentials: _options.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }

        public int Register(string username, string email, string password)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Checks if user credentials are valid
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GetIdentityAsync(string username, string password)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, "User logged in.");
                return new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"User {username} locked out.");
                return null;
            }
            else
            {
                _logger.LogWarning(2, $"Invalid user login for {username}");
                return null;
            }
        }


        /// <summary>
        /// Get a user. 
        /// </summary>
        /// <param name="userClaim"></param>
        /// <returns></returns>
        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal userClaim)
        {
            return _userManager.GetUserAsync(userClaim);
        }

   
    }
}
