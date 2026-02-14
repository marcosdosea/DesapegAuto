using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Areas.Identity.Data;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class ConcessionariaControllerTests
    {
        private static ConcessionariaController controller = null!;
        private static Mock<IConcessionariaService> mockConcessionariaService = null!;
        private static Mock<UserManager<ApplicationUser>> mockUserManager = null!;
        private static IMapper mapper = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockConcessionariaService = new Mock<IConcessionariaService>();
            mockUserManager = CreateUserManagerMock();
            mapper = new MapperConfiguration(cfg => cfg.AddProfile(new ConcessionariaProfile())).CreateMapper();

            mockConcessionariaService.Setup(s => s.Get(7)).Returns(new Concessionaria
            {
                Id = 7,
                Nome = "Concessionaria 7",
                Cnpj = "12.345.678/0001-90",
                Email = "c7@email.com",
                Telefone = "11999990000",
                Senha = "ab1234",
                Endereco = "Rua 1"
            });

            controller = new ConcessionariaController(
                mockConcessionariaService.Object,
                mapper,
                mockUserManager.Object);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [TestMethod]
        public async Task CreatePost_QuandoUserManagerFalha_DeveFazerRollbackDaConcessionaria()
        {
            mockConcessionariaService.Setup(s => s.Create(It.IsAny<Concessionaria>()))
                .Callback<Concessionaria>(c => c.Id = 50)
                .Returns(50);

            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Falha ao criar usuário" }));

            var vm = GetValidConcessionariaViewModel();

            var result = await controller.Create(vm);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            mockConcessionariaService.Verify(s => s.Delete(50), Times.Once);
        }

        [TestMethod]
        public void MeuCadastroGet_SemClaimConcessionaria_DeveRetornarForbid()
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = controller.MeuCadastro();

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void MeuCadastroPost_ComIdDiferenteDoClaim_DeveRetornarForbid()
        {
            var claims = new[] { new Claim("ConcessionariaId", "7") };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };

            var vm = GetValidConcessionariaViewModel();
            vm.Id = 8;

            var result = controller.MeuCadastro(vm);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
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

        private static ConcessionariaViewModel GetValidConcessionariaViewModel()
        {
            return new ConcessionariaViewModel
            {
                Id = 1,
                Nome = "Concessionaria Nova",
                Cnpj = "12.345.678/0001-90",
                Email = "nova@concessionaria.com",
                Telefone = "11988887777",
                Senha = "abc123",
                ConfirmarSenha = "abc123",
                Endereco = "Rua das Flores"
            };
        }
    }
}
