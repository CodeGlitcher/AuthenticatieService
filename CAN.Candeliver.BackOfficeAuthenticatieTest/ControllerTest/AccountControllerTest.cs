using CAN.Candeliver.BackOfficeAuthenticatie.Controllers;
using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using CAN.Candeliver.BackOfficeAuthenticatie.Models.AccountViewModels;
using CAN.Candeliver.BackOfficeAuthenticatie.Security;
using CAN.Candeliver.BackOfficeAuthenticatie.Services;
using CAN.Candeliver.BackOfficeAuthenticatieTest.Provider;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAN.Candeliver.BackOfficeAuthenticatieTest.ControllerTest
{
    [TestClass]
    public class AccountControllerTest
    {

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task TestCreateUser()
        {
            var logMock = new Mock<ILogger<AccountController>>();
            var accountServiceMock = new Mock<IAccountService>(MockBehavior.Strict);
            var optionsMock = MockProvider.CreateProviderOptionsMock();


            accountServiceMock.Setup(e => e.RegisterAsync("Rob", "tset123", "Sales")).ReturnsAsync(new ApplicationUser());

            var controller = new AccountController(logMock.Object, optionsMock.Object, accountServiceMock.Object);

            var registermodel = new RegisterViewModel()
            {
                ConfirmPassword = "tset123",
                Password = "tset123",
                Role = "Sales",
                UserName = "Rob"
            };
            var result = await controller.Register(registermodel);

            accountServiceMock.Verify(e => e.RegisterAsync("Rob", "tset123", "Sales"), Times.Once);

            Assert.IsInstanceOfType(result, typeof(JsonResult));

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task TestCreateUserFail()
        {
            var logMock = new Mock<ILogger<AccountController>>();
            var accountServiceMock = new Mock<IAccountService>(MockBehavior.Strict);
            var optionsMock = MockProvider.CreateProviderOptionsMock();


            accountServiceMock.Setup(e => e.RegisterAsync("Rob", "tset123", "Sales")).ReturnsAsync(null);

            var controller = new AccountController(logMock.Object, optionsMock.Object, accountServiceMock.Object);

            var registermodel = new RegisterViewModel()
            {
                ConfirmPassword = "tset123",
                Password = "tset123",
                Role = "Sales",
                UserName = "Rob"
            };
            var result = await controller.Register(registermodel);

            accountServiceMock.Verify(e => e.RegisterAsync("Rob", "tset123", "Sales"), Times.Once);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        }



        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task TestLoginUser()
        {
            var logMock = new Mock<ILogger<AccountController>>();
            var accountServiceMock = new Mock<IAccountService>(MockBehavior.Strict);
            var optionsMock = MockProvider.CreateProviderOptionsMock();

            var applicationUser = new ApplicationUser() { UserName = "Rob" };
            accountServiceMock.Setup(e => e.GetIdentityAsync("Rob", "tset123")).ReturnsAsync(new ClaimsIdentity());
            accountServiceMock.Setup(e => e.GetUserAsync("Rob")).Returns(applicationUser);
            accountServiceMock.Setup(e => e.CreateJwtTokenForUserAsync(applicationUser)).ReturnsAsync("SuperEncodedToken");

            var controller = new AccountController(logMock.Object, optionsMock.Object, accountServiceMock.Object);

            var loginModel = new LoginViewModel()
            {
                Password = "tset123",
                UserName = "Rob"
            };
            var result = await controller.Login(loginModel);

            accountServiceMock.Verify(e => e.CreateJwtTokenForUserAsync(applicationUser), Times.Once);

            Assert.IsInstanceOfType(result, typeof(JsonResult));

            var json = result as JsonResult;
            var loginResult = json.Value as LoginResult;
            Assert.AreEqual("SuperEncodedToken", loginResult.Access_Token);
        }
                
    }
}
