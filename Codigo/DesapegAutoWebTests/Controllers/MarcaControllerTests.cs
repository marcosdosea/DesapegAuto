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
    public class MarcaControllerTests
    {
        private static MarcaController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockService = new Mock<IMarcaService>();
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new MarcaProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll())
                .Returns(GetTestMarcas());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetMarca());
            mockService.Setup(service => service.Edit(It.IsAny<Marca>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Marca>()))
                .Returns(4); // Retorna o ID da nova marca criada
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            controller = new MarcaController(mockService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTestValido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<MarcaViewModel>));

            var listaMarcas = (IEnumerable<MarcaViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaMarcas.Count());
        }

        [TestMethod()]
        public void DetailsTestValido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MarcaViewModel));
            MarcaViewModel marcaModel = (MarcaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Ford", marcaModel.Nome);
        }

        [TestMethod()]
        public void CreateTestGetValido()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTestPostValido()
        {
            // Act
            var result = controller.Create(GetNewMarca());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTestPostInvalido()
        {
            // Arrange
            controller.ModelState.AddModelError("Nome", "Campo requerido");

            // Act
            var result = controller.Create(GetNewMarca());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MarcaViewModel));
        }


        [TestMethod()]
        public void EditTestGetValido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MarcaViewModel));
            MarcaViewModel marcaModel = (MarcaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Ford", marcaModel.Nome);
        }

        [TestMethod()]
        public void EditTestPostValido()
        {
            // Act
            var result = controller.Edit(GetTargetMarcaModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTestGetValido()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MarcaViewModel));
            MarcaViewModel marcaModel = (MarcaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Ford", marcaModel.Nome);
        }

        [TestMethod()]
        public void DeleteTestPostValido()
        {
            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        // Métodos auxiliares para gerar dados de teste
        private MarcaViewModel GetNewMarca()
        {
            return new MarcaViewModel
            {
                Id = 4,
                Nome = "Honda"
            };
        }

        private static Marca GetTargetMarca()
        {
            return new Marca
            {
                Id = 1,
                Nome = "Ford"
            };
        }

        private MarcaViewModel GetTargetMarcaModel()
        {
            return new MarcaViewModel
            {
                Id = 1,
                Nome = "Ford"
            };
        }

        private static IEnumerable<Marca> GetTestMarcas()
        {
            return new List<Marca>
            {
                new Marca { Id = 1, Nome = "Ford" },
                new Marca { Id = 2, Nome = "Chevrolet" },
                new Marca { Id = 3, Nome = "Toyota" }
            };
        }
    
    }
}