using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Http;
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
        private static Mock<IConcessionariaService> mockConcessionariaService = null!;
        private static Mock<IModeloService> mockModeloService = null!;
        private static Mock<IMarcaService> mockMarcaService = null!;
        private static IMapper mapper = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockAnuncioService = new Mock<IAnuncioService>();
            mockVeiculoService = new Mock<IVeiculoService>();
            mockConcessionariaService = new Mock<IConcessionariaService>();
            mockModeloService = new Mock<IModeloService>();
            mockMarcaService = new Mock<IMarcaService>();
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
            mockConcessionariaService.Setup(s => s.GetAll()).Returns(new List<Concessionaria>());
            mockConcessionariaService.Setup(s => s.Get(It.IsAny<int>())).Returns(new Concessionaria { Id = 1, Nome = "Concessionária Teste" });
            mockModeloService.Setup(s => s.GetAll()).Returns(new List<Modelo> { new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1 } });
            mockMarcaService.Setup(s => s.GetAll()).Returns(new List<Marca> { new Marca { Id = 1, Nome = "Toyota" } });
            
            mockModeloService.Setup(s => s.Get(It.IsAny<int>())).Returns(new Modelo { Id = 1, Nome = "Corolla" });
            mockMarcaService.Setup(s => s.Get(It.IsAny<int>())).Returns(new Marca { Id = 1, Nome = "Toyota" });

            controller = new AnuncioController(
                mockAnuncioService.Object,
                mockVeiculoService.Object,
                mockModeloService.Object,
                mockMarcaService.Object,
                mockConcessionariaService.Object,
                mapper);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
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
        public void DetailsTest_DeveIncrementarVisualizacoesEEditarAnuncio()
        {
            var anuncio = new Anuncio
            {
                Id = 10,
                IdVeiculo = 1,
                StatusAnuncio = "D",
                DataPublicacao = System.DateTime.Now,
                Visualizacoes = 7,
                Descricao = "teste",
                Opcionais = "ar"
            };
            mockAnuncioService.Setup(s => s.Get(10)).Returns(anuncio);

            var result = controller.Details(10);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            mockAnuncioService.Verify(s => s.Edit(It.Is<Anuncio>(a => a.Id == 10 && a.Visualizacoes == 8)), Times.Once);
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
            var viewModel = new AnuncioCreateViewModel
            {
                IdConcessionaria = 1,
                IdMarca = 1,
                IdModelo = 1,
                Ano = 2020,
                Cor = "Preto",
                Quilometragem = 15000,
                Preco = 90000.00m,
                Placa = "TES-0001",
                Descricao = "Anúncio de teste",
                OpcionaisSelecionados = new List<string> { "Ar Condicionado" }
            };
            var result = controller.Create(viewModel);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_QuandoServicoFalha_DeveRetornarViewComErro()
        {
            mockVeiculoService.Setup(s => s.Create(It.IsAny<Veiculo>()))
                .Throws(new ServiceException("Falha ao criar veículo"));

            var viewModel = new AnuncioCreateViewModel
            {
                IdConcessionaria = 1,
                IdMarca = 1,
                IdModelo = 1,
                Ano = 2022,
                Cor = "Prata",
                Quilometragem = 10000,
                Preco = 120000.00m,
                Placa = "TES-2022",
                Descricao = "Falha esperada",
                OpcionaisSelecionados = new List<string> { "Sensor" }
            };

            var result = controller.Create(viewModel);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(AnuncioCreateViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
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
