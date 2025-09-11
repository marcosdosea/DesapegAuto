using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class AnuncioControllerTests
    {
        private static AnuncioController controller = null!;
        private static Mock<IAnuncioService> mockAnuncioService = null!;
        private static Mock<IVeiculoService> mockVeiculoService = null!;
        private static Mock<IVendaService> mockVendaService = null!;
        private static IMapper mapper = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockAnuncioService = new Mock<IAnuncioService>();
            mockVeiculoService = new Mock<IVeiculoService>();
            mockVendaService = new Mock<IVendaService>();
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AnuncioProfile());
                cfg.AddProfile(new VeiculoProfile());
            }).CreateMapper();

            mockAnuncioService.Setup(s => s.GetAll()).Returns(GetTestAnuncios());
            mockAnuncioService.Setup(s => s.Get(1)).Returns(GetTargetAnuncio());
            mockAnuncioService.Setup(s => s.Create(It.IsAny<Anuncio>())).Returns(3);
            mockAnuncioService.Setup(s => s.Edit(It.IsAny<Anuncio>())).Verifiable();
            mockAnuncioService.Setup(s => s.Delete(It.IsAny<int>())).Verifiable();

            mockVeiculoService.Setup(s => s.Get(It.IsAny<int>())).Returns(GetTargetVeiculo());
            mockVeiculoService.Setup(s => s.GetAll()).Returns(new List<Veiculo> { GetTargetVeiculo() });
            mockVendaService.Setup(s => s.GetAll()).Returns(new List<Venda>());

            controller = new AnuncioController(
                mockAnuncioService.Object,
                mockVeiculoService.Object,
                mockVendaService.Object,
                mapper);
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AnuncioViewModel>));
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(2, lista.Count());
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AnuncioViewModel));
            var model = (AnuncioViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
        }

        [TestMethod]
        public void CreateTest_Get()
        {
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Post_Valido()
        {
            var viewModel = new AnuncioViewModel { Id = 3, IdVeiculo = 1 };
            var result = controller.Create(viewModel);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void DeleteTest_Get()
        {
            var result = controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AnuncioViewModel));
        }

        [TestMethod]
        public void DeleteTest_Post()
        {
            var viewModel = new AnuncioViewModel { Id = 1 };
            var result = controller.Delete(1, viewModel);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        private static IEnumerable<Anuncio> GetTestAnuncios()
        {
            return new List<Anuncio>
            {
                new Anuncio { Id = 1, IdVeiculo = 1, StatusAnuncio = "D", DataPublicacao = System.DateTime.Now, Visualizacoes = 10, Descricao = "desc1", Opcionais = "opt1" },
                new Anuncio { Id = 2, IdVeiculo = 1, StatusAnuncio = "D", DataPublicacao = System.DateTime.Now, Visualizacoes = 5, Descricao = "desc2", Opcionais = "opt2" }
            };
        }
        private static Anuncio GetTargetAnuncio()
        {
            return new Anuncio { Id = 1, IdVeiculo = 1, StatusAnuncio = "D", DataPublicacao = System.DateTime.Now, Visualizacoes = 10, Descricao = "desc1", Opcionais = "opt1" };
        }
        private static Veiculo GetTargetVeiculo()
        {
            return new Veiculo { Id = 1, Placa = "ABC-1234", Ano = 2020, Cor = "Preto", Quilometragem = 35000, Preco = 20000.00m, IdConcessionaria = 1, IdModelo = 1, IdMarca = 1 };
        }
    }
}
