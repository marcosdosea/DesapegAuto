using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using DesapegAutoWeb.Controllers;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass()]
    public class VeiculoControllerTests
    {
        private static VeiculoController controller;
        private static Mock<IVeiculoService> mockService;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            mockService = new Mock<IVeiculoService>();
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new VeiculoProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll())
                .Returns(GetTestVeiculos());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetVeiculo());
            mockService.Setup(service => service.Edit(It.IsAny<Veiculo>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Veiculo>()))
                .Returns(4);
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            // Configuração para simular erro no Delete
            mockService.Setup(service => service.Delete(99))
                .Throws(new ServiceException("Veículo não encontrado"));

            controller = new VeiculoController(mockService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTestValido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<VeiculoViewModel>));

            var listaVeiculos = (IEnumerable<VeiculoViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaVeiculos.Count());
        }

        [TestMethod()]
        public void DetailsTestValido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VeiculoViewModel));
            VeiculoViewModel veiculoModel = (VeiculoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("ABC-1234", veiculoModel.Placa);
            Assert.AreEqual(20000.00m, veiculoModel.Preco);
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
        public void CreateTestPostValid()
        {
            // Act
            var result = controller.Create(GetNewVeiculo());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTestPostInvalid()
        {
            // Arrange
            controller.ModelState.AddModelError("Placa", "Campo requerido");

            // Act
            var result = controller.Create(GetNewVeiculo());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VeiculoViewModel));
        }

        [TestMethod()]
        public void EditTestGetValid()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VeiculoViewModel));
            VeiculoViewModel veiculoModel = (VeiculoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("ABC-1234", veiculoModel.Placa);
            Assert.AreEqual(20000.00m, veiculoModel.Preco);
        }

        [TestMethod()]
        public void EditTestPostValid()
        {
            // Act
            var result = controller.Edit(1, GetTargetVeiculoModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTestGetValid()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VeiculoViewModel));
            VeiculoViewModel veiculoModel = (VeiculoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("ABC-1234", veiculoModel.Placa);
            Assert.AreEqual(20000.00m, veiculoModel.Preco);
        }

        [TestMethod()]
        public void DeleteTestPostValid()
        {
            // Act
            var result = controller.Delete(1, GetTargetVeiculoModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            // Arrange
            var veiculoVm = new VeiculoViewModel { Id = 1, Placa = "ABC-1234 Modificado", Ano = 2020, Cor = "Azul", Quilometragem = 40000, Preco = 22000.00m };

            // Act
            var result = controller.Edit(1, veiculoVm);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(s => s.Edit(It.IsAny<Veiculo>()), Times.Once);
        }

        [TestMethod]
        public void Create_Post_ModelStateInvalida_RetornaViewComModel()
        {
            // Arrange
            controller.ModelState.AddModelError("Placa", "A placa do veículo é obrigatória");
            var novoVeiculoVM = new VeiculoViewModel { Id = 5, Placa = string.Empty, Ano = 2023, Cor = "Preto", Quilometragem = 0, Preco = 50000.00m };

            // Act
            var result = controller.Create(novoVeiculoVM);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(novoVeiculoVM, viewResult.Model);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Delete_Post_QuandoServicoLancaExcecao_RetornaViewComErro()
        {
            // Arrange
            var veiculoVM = new VeiculoViewModel { Id = 99, Placa = "XXX-9999", Ano = 2020, Cor = "Cinza", Quilometragem = 50000, Preco = 25000.00m };

            // Act & Assert - Método Delete lança exceção, verificamos o comportamento
            var exception = Assert.ThrowsException<ServiceException>(() => controller.Delete(99, veiculoVM));
            Assert.IsTrue(exception.Message.Contains("Veículo não encontrado"));
            mockService.Verify(s => s.Delete(99), Times.Once);
        }

        private VeiculoViewModel GetNewVeiculo()
        {
            return new VeiculoViewModel
            {
                Id = 4,
                Placa = "XYZ-9876",
                Ano = 2022,
                Cor = "Prata",
                Quilometragem = (int)50000.0,
                Preco = 45000.00m
            };
        }

        private static Veiculo GetTargetVeiculo()
        {
            return new Veiculo
            {
                Id = 1,
                Placa = "ABC-1234",
                Ano = 2020,
                Cor = "Preto",
                Quilometragem = (int)35000.0,
                Preco = 20000.00m
            };
        }

        private VeiculoViewModel GetTargetVeiculoModel()
        {
            return new VeiculoViewModel
            {
                Id = 1,
                Placa = "ABC-1234",
                Ano = 2020,
                Cor = "Preto",
                Quilometragem = (int)35000.0,
                Preco = 20000.00m
            };
        }

        private static IEnumerable<Veiculo> GetTestVeiculos()
        {
            return new List<Veiculo>
            {
                new Veiculo { Id = 1, Placa = "ABC-1234", Ano = 2020, Cor = "Preto", Quilometragem = (int)35000.0, Preco = 20000.00m },
                new Veiculo { Id = 2, Placa = "DEF-5678", Ano = 2021, Cor = "Branco", Quilometragem = (int) 25000.0, Preco = 25000.00m },
                new Veiculo { Id = 3, Placa = "GHI-9012", Ano = 2019, Cor = "Vermelho", Quilometragem = (int) 60000.0, Preco = 18000.00m }
            };
        }
    }
}