using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DesapegAutoWeb.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace DesapegAutoWeb.Controllers.Tests
{
    [TestClass()]
    public class CategoriaControllerTests
    {
        private static CategoriaController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            var mockService = new Mock<ICategoriaService>();
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new CategoriaProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll())
                .Returns(GetTestCategorias());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetCategoria());
            mockService.Setup(service => service.Edit(It.IsAny<Categoria>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Categoria>()))
                .Returns(4);
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            controller = new CategoriaController(mockService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTestValido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<CategoriaViewModel>));

            var listaCategorias = (IEnumerable<CategoriaViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaCategorias.Count());
        }

        [TestMethod()]
        public void DetailsTestValido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CategoriaViewModel));
            CategoriaViewModel categoriaModel = (CategoriaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("SUV", categoriaModel.Nome);
        }

        [TestMethod()]
        public void CreateTestGetValido()
        {
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTestPostValido()
        {
            var result = controller.Create(GetNewCategoria());
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTestPostInvalido()
        {
            controller.ModelState.AddModelError("Nome", "Campo requerido");
            var result = controller.Create(GetNewCategoria());
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CategoriaViewModel));
        }

        [TestMethod()]
        public void EditTestGetValido()
        {
            var result = controller.Edit(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CategoriaViewModel));
            CategoriaViewModel categoriaModel = (CategoriaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("SUV", categoriaModel.Nome);
        }

        [TestMethod()]
        public void EditTestPostValido()
        {
            var result = controller.Edit(GetTargetCategoriaModel());
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTestGetValido()
        {
            var result = controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CategoriaViewModel));
            CategoriaViewModel categoriaModel = (CategoriaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("SUV", categoriaModel.Nome);
        }

        [TestMethod()]
        public void DeleteTestPostValido()
        {
            var result = controller.DeleteConfirmed(1);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private CategoriaViewModel GetNewCategoria()
        {
            return new CategoriaViewModel { Id = 4, Nome = "Crossover" };
        }

        private static Categoria GetTargetCategoria()
        {
            return new Categoria { Id = 1, Nome = "SUV" };
        }

        private CategoriaViewModel GetTargetCategoriaModel()
        {
            return new CategoriaViewModel { Id = 1, Nome = "SUV" };
        }

        private static IEnumerable<Categoria> GetTestCategorias()
        {
            return new List<Categoria>
            {
                new Categoria { Id = 1, Nome = "SUV" },
                new Categoria { Id = 2, Nome = "Sedan" },
                new Categoria { Id = 3, Nome = "Hatch" }
            };
        }
    }
}
