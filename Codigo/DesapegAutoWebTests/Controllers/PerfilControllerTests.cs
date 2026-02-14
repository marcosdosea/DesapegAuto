using DesapegAutoWeb.Areas.Identity.Data;
using DesapegAutoWeb.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class PerfilControllerTests
    {
        private static PerfilController controller = null!;
        private static Mock<UserManager<ApplicationUser>> mockUserManager = null!;
        private static Mock<SignInManager<ApplicationUser>> mockSignInManager = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockUserManager = CreateUserManagerMock();
            mockSignInManager = CreateSignInManagerMock(mockUserManager);

            controller = new PerfilController(mockUserManager.Object, mockSignInManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TestMethod]
        public async Task Index_QuandoUsuarioNaoExiste_DeveRetornarNotFound()
        {
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns("abc");

            var result = await controller.Index();

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Index_QuandoUsuarioExiste_DeveRetornarViewComModelo()
        {
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Email = "user@email.com",
                    UserName = "usuario.teste",
                    PhoneNumber = "11999998888",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = true
                });

            var result = await controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(PerfilViewModel));
            var model = (PerfilViewModel)viewResult.Model!;
            Assert.AreEqual("user@email.com", model.Email);
            Assert.AreEqual("usuario.teste", model.UserName);
            Assert.IsTrue(model.EmailConfirmed);
        }

        [TestMethod]
        public void Publico_DeveRetornarView()
        {
            var result = controller.Publico();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(Mock<UserManager<ApplicationUser>> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }
    }
}
