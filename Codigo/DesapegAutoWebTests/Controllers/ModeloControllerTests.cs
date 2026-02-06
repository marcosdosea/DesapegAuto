using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class ModeloControllerTests
    {
        private static ModeloController controller = null!;
        private static Mock<IModeloService> mockModeloService = null!;
        private static Mock<IMarcaService> mockMarcaService = null!;
        private static Mock<ICategoriaService> mockCategoriaService = null!;
        private static IMapper mapper = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockModeloService = new Mock<IModeloService>();
            mockMarcaService = new Mock<IMarcaService>();
            mockCategoriaService = new Mock<ICategoriaService>();

            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ModeloProfile());
            }).CreateMapper();

            mockModeloService.Setup(s => s.GetAll()).Returns(GetTestModelos());
            mockModeloService.Setup(s => s.Get(1)).Returns(GetTargetModelo());
            mockModeloService.Setup(s => s.Create(It.IsAny<Modelo>())).Returns(3);
            mockModeloService.Setup(s => s.Edit(It.IsAny<Modelo>())).Verifiable();
            mockModeloService.Setup(s => s.Delete(It.IsAny<int>())).Verifiable();
            mockModeloService.Setup(service => service.Delete(99)).Throws(new ServiceException("Modelo nao encontrado"));

            mockMarcaService.Setup(s => s.GetAll()).Returns(new List<Marca>
            {
                new Marca { Id = 1, Nome = "Toyota" },
                new Marca { Id = 2, Nome = "Honda" }
            });

            mockCategoriaService.Setup(s => s.GetAll()).Returns(new List<Categoria>
            {
                new Categoria { Id = 1, Nome = "Sedan" },
                new Categoria { Id = 2, Nome = "SUV" }
            });

            controller = new ModeloController(
                mockModeloService.Object,
                mockMarcaService.Object,
                mockCategoriaService.Object,
                mapper);

            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<ModeloViewModel>));
            var lista = (IEnumerable<ModeloViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(2, lista.Count());
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ModeloViewModel));
            var model = (ModeloViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
        }

        [TestMethod]
        public void CreateTest_Get_RedirecionaParaIndex()
        {
            var result = controller.Create();

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Valido()
        {
            var modeloVm = new ModeloViewModel { Id = 3, Nome = "Novo Modelo", IdMarca = 1, IdCategoria = 1, Versoes = "X" };
            var result = controller.Create(modeloVm);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            mockModeloService.Verify(s => s.Create(It.IsAny<Modelo>()), Times.Once);
        }

        [TestMethod]
        public void DeleteTest_Get()
        {
            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ModeloViewModel));
        }

        [TestMethod]
        public void DeleteTest_Post()
        {
            var result = controller.DeleteConfirmed(1);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            mockModeloService.Verify(s => s.Delete(1), Times.Once);
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            var modeloVm = new ModeloViewModel { Id = 1, Nome = "Corolla Modificado", IdMarca = 1, IdCategoria = 1 };

            var result = controller.Edit(1, modeloVm);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockModeloService.Verify(s => s.Edit(It.IsAny<Modelo>()), Times.Once);
        }

        [TestMethod]
        public void Create_Post_ModelStateInvalida_RedirecionaParaIndex()
        {
            controller.ModelState.AddModelError("Nome", "O nome do modelo e obrigatorio");
            var novoModeloVM = new ModeloViewModel { Id = 3, Nome = string.Empty, IdMarca = 1, IdCategoria = 1 };

            var result = controller.Create(novoModeloVM);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Delete_Post_QuandoServicoLancaExcecao_RedirecionaParaIndex()
        {
            var result = controller.DeleteConfirmed(99);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            mockModeloService.Verify(s => s.Delete(99), Times.Once);
        }

        private static IEnumerable<Modelo> GetTestModelos()
        {
            return new List<Modelo>
            {
                new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1, IdCategoria = 1, Categoria = "Sedan" },
                new Modelo { Id = 2, Nome = "Civic", IdMarca = 2, IdCategoria = 1, Categoria = "Sedan" }
            };
        }

        private static Modelo GetTargetModelo()
        {
            return new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1, IdCategoria = 1, Categoria = "Sedan" };
        }
    }
}
