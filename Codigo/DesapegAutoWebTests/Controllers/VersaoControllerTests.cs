using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass()]
    public class VersaoControllerTests
    {
        private static VersaoController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockVersaoService = new Mock<IVersaoService>();
            var mockModeloService = new Mock<IModeloService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new VersaoProfile())).CreateMapper();

            mockVersaoService.Setup(service => service.GetAll())
                .Returns(GetTestVersoes());
            mockVersaoService.Setup(service => service.Get(1))
                .Returns(GetTargetVersao());
            mockVersaoService.Setup(service => service.Edit(It.IsAny<Versao>()))
                .Verifiable();
            mockVersaoService.Setup(service => service.Create(It.IsAny<Versao>()))
                .Returns(4); // Retorna o ID da nova versão
            mockVersaoService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            // Mock para o serviço de Modelo, necessário para o dropdown
            mockModeloService.Setup(service => service.GetAll())
                .Returns(new List<Modelo>()); // Retorna uma lista vazia, pois o conteúdo não importa no teste

            controller = new VersaoController(mockVersaoService.Object, mockModeloService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<VersaoViewModel>));

            var listaVersoes = (List<VersaoViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaVersoes.Count);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VersaoViewModel));
            VersaoViewModel versaoModel = (VersaoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("GLI", versaoModel.Nome);
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller.Create();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valido()
        {
            // Act
            var result = controller.Create(GetNewVersaoViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void EditTest_Get_Valido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VersaoViewModel));
            VersaoViewModel versaoModel = (VersaoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("GLI", versaoModel.Nome);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Act
            var result = controller.Edit(1, GetTargetVersaoViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Get_Valido()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VersaoViewModel));
            VersaoViewModel versaoModel = (VersaoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("GLI", versaoModel.Nome);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valido()
        {
            // Act
            var result = controller.Delete(1, GetTargetVersaoViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private static VersaoViewModel GetNewVersaoViewModel()
        {
            return new VersaoViewModel { Id = 4, Nome = "Highline", IdModelo = 1 };
        }

        private static Versao GetTargetVersao()
        {
            return new Versao { Id = 1, Nome = "GLI", IdModelo = 1 };
        }

        private static VersaoViewModel GetTargetVersaoViewModel()
        {
            return new VersaoViewModel { Id = 1, Nome = "GLI", IdModelo = 1 };
        }

        private static IEnumerable<Versao> GetTestVersoes()
        {
            return new List<Versao>
            {
                new() { Id = 1, Nome = "GLI", IdModelo = 1 },
                new() { Id = 2, Nome = "Comfortline", IdModelo = 1 },
                new() { Id = 3, Nome = "XEi", IdModelo = 2 }
            };
        }
    }
}