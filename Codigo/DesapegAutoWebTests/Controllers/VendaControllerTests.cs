using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass()]
    public class VendaControllerTests
    {
        private static VendaController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            var mockVendaService = new Mock<IVendaService>();
            var mockConcessionariaService = new Mock<IConcessionariaService>();
            var mockPessoaService = new Mock<IPessoaService>(); // Usando Pessoa em vez de Usuario

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new VendaProfile())).CreateMapper();

            mockVendaService.Setup(service => service.GetAllDTO())
                .Returns(GetTestVendasDTO());
            mockVendaService.Setup(service => service.Get(1))
                .Returns(GetTargetVenda());
            mockVendaService.Setup(service => service.Create(It.IsAny<Venda>()))
                .Returns(3);
            mockVendaService.Setup(service => service.Edit(It.IsAny<Venda>()))
                .Verifiable();
            mockVendaService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            // Mock dos serviços de dependência para os dropdowns
            mockConcessionariaService.Setup(service => service.GetAll()).Returns(new List<Concessionaria>());
            mockPessoaService.Setup(service => service.GetAll()).Returns(new List<Pessoa>());

            controller = new VendaController(
                mockVendaService.Object,
                mockConcessionariaService.Object,
                mockPessoaService.Object,
                mapper);
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<VendaDTO>));
            var listaVendas = (IEnumerable<VendaDTO>)viewResult.ViewData.Model;
            Assert.AreEqual(2, listaVendas.Count());
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(VendaViewModel));
            VendaViewModel vendaModel = (VendaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(50000.00m, vendaModel.ValorFinal);
        }

        [TestMethod()]
        public void CreateTest_Post_Valido()
        {
            // Act
            var result = controller.Create(GetNewVendaViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Act
            var result = controller.Edit(1, GetTargetVendaViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valido()
        {
            // Act
            var result = controller.Delete(1, GetTargetVendaViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }


        private static VendaViewModel GetNewVendaViewModel()
        {
            return new VendaViewModel
            {
                Id = 3,
                DataVenda = DateTime.Now,
                ValorFinal = 90000.00m,
                FormaPagamento = "Consórcio",
                IdConcessionaria = 1,
                IdPessoa = 2 
            };
        }

        private static Venda GetTargetVenda()
        {
            return new Venda
            {
                Id = 1,
                DataVenda = DateTime.Now.AddDays(-1),
                ValorFinal = 50000.00m,
                FormaPagamento = "Financiamento",
                IdConcessionaria = 1,
                IdPessoa = 1
            };
        }

        private static VendaViewModel GetTargetVendaViewModel()
        {
            return new VendaViewModel
            {
                Id = 1,
                DataVenda = DateTime.Now.AddDays(-1),
                ValorFinal = 50000.00m,
                FormaPagamento = "Financiamento",
                IdConcessionaria = 1,
                IdPessoa = 1
            };
        }

        private static IEnumerable<VendaDTO> GetTestVendasDTO()
        {
            return new List<VendaDTO>
            {
                new() { Id = 1, DataVenda = DateTime.Now, ValorFinal = 50000.00m, FormaPagamento = "Financiamento", NomeConcessionaria = "AutoFácil", NomePessoa = "João Silva" },
                new() { Id = 2, DataVenda = DateTime.Now.AddDays(-10), ValorFinal = 75000.00m, FormaPagamento = "À Vista", NomeConcessionaria = "VendeCar", NomePessoa = "Maria Oliveira" }
            };
        }
    }
}