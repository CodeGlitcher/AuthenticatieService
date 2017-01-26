using CAN.Candeliver.BackOfficeAuthenticatie.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAN.Candeliver.BackOfficeAuthenticatieTest.Provider
{
    public class MockProvider
    {

        public static Mock<IOptions<TokenProviderOptions>> CreateProviderOptionsMock()
        {
            var mock = new Mock<IOptions<TokenProviderOptions>>(MockBehavior.Strict);
            var secretKey = "SUPERsecretkey_123456789";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var options = new TokenProviderOptions();

            

            options.Audience = "ExampleAudience";
            options.Issuer = "ExampleIssuer";
            options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            options.Expiration = TimeSpan.FromMinutes(30);

            mock.Setup(e => e.Value).Returns(options);
            return mock;
        }
    }
}
