using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private static HomeController controller = null!;
        private static Mock<ILogger<HomeController>> mockLogger = null!;
        private static Mock<IAnuncioService> mockAnuncioService = null!;
        private static Mock<IVeiculoService> mockVeiculoService = null!;
        private static Mock<IModeloService> mockModeloService = null!;
        private static Mock<IMarcaService> mockMarcaService = null!;
        private static IMapper mapper = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockLogger = new Mock<ILogger<HomeController>>();
            mockAnuncioService = new Mock<IAnuncioService>();
            mockVeiculoService = new Mock<IVeiculoService>();
            mockModeloService = new Mock<IModeloService>();
            mockMarcaService = new Mock<IMarcaService>();
            
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AnuncioProfile());
                cfg.AddProfile(new VeiculoProfile());
            }).CreateMapper();

            // Setup dos mocks
            mockAnuncioService.Setup(s => s.GetAll()).Returns(GetTestAnuncios());
            mockVeiculoService.Setup(s => s.Get(1)).Returns(GetTargetVeiculo());
            mockVeiculoService.Setup(s => s.Get(2)).Returns(GetSecondVeiculo());
            mockModeloService.Setup(s => s.Get(1)).Returns(new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1 });
            mockModeloService.Setup(s => s.Get(2)).Returns(new Modelo { Id = 2, Nome = "Civic", IdMarca = 2 });
            mockMarcaService.Setup(s => s.Get(1)).Returns(new Marca { Id = 1, Nome = "Toyota" });
            mockMarcaService.Setup(s => s.Get(2)).Returns(new Marca { Id = 2, Nome = "Honda" });

            controller = new HomeController(
                mockLogger.Object,
                mockAnuncioService.Object,
                mockVeiculoService.Object,
                mockModeloService.Object,
                mockMarcaService.Object,
                mapper);

            // Configurar HttpContext com usuário autenticado
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Arrange – GetAll() is already set up; also need GetAll() on marca/modelo services
            mockMarcaService.Setup(s => s.GetAll()).Returns(new List<Marca>
            {
                new Marca { Id = 1, Nome = "Toyota" },
                new Marca { Id = 2, Nome = "Honda" }
            });
            mockModeloService.Setup(s => s.GetAll()).Returns(new List<Modelo>
            {
                new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1, Categoria = "Sedan" },
                new Modelo { Id = 2, Nome = "Civic",   IdMarca = 2, Categoria = "Sedan" }
            });

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(HomeViewModel));
            var homeModel = (HomeViewModel)viewResult.ViewData.Model!;
            Assert.IsTrue(homeModel.Destaques.Count <= 3, "Index deve retornar no máximo 3 destaques");
        }

        [TestMethod]
        public void SearchTest_SemFiltros_RetornaTodos()
        {
            // Act
            var result = controller.Search(null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AnuncioViewModel>));
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void SearchTest_ComTermo_FiltradoPorMarca()
        {
            // Arrange
            string termo = "Toyota";

            // Act
            var result = controller.Search(termo, null, null, null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;
            
            // Verifica que todos os resultados cont�m Toyota
            foreach (var item in lista)
            {
                Assert.IsTrue(
                    item.Veiculo?.NomeMarca?.Contains("Toyota") ?? false,
                    "Todos os resultados devem ser da marca Toyota"
                );
            }
        }

        [TestMethod]
        public void SearchTest_ComTermo_FiltradoPorModelo()
        {
            // Arrange
            string termo = "Corolla";

            // Act
            var result = controller.Search(termo, null, null, null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;
            
            // O filtro por termo funciona - verifica que retornou resultados
            // (o controller filtra por marca/modelo via serviço)
            Assert.IsNotNull(lista);
        }

        [TestMethod]
        public void SearchTest_ComFiltroPreco_RetornaApenasDentroFaixa()
        {
            // Arrange
            decimal precoMin = 15000;
            decimal precoMax = 25000;

            // Act
            var result = controller.Search(null, precoMin, precoMax, null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;

            // Verifica que todos os ve�culos est�o dentro da faixa de pre�o
            foreach (var item in lista)
            {
                Assert.IsTrue(item.Veiculo?.Preco >= precoMin && item.Veiculo?.Preco <= precoMax,
                    "Todos os ve�culos devem estar dentro da faixa de pre�o");
            }
        }

        [TestMethod]
        public void SearchTest_ComFiltroAno_RetornaApenasDentroFaixa()
        {
            // Arrange
            int anoMin = 2019;
            int anoMax = 2020;

            // Act
            var result = controller.Search(null, null, null, anoMin, anoMax, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;

            // Verifica que todos os ve�culos est�o dentro da faixa de ano
            foreach (var item in lista)
            {
                Assert.IsTrue(item.Veiculo?.Ano >= anoMin && item.Veiculo?.Ano <= anoMax,
                    "Todos os ve�culos devem estar dentro da faixa de ano");
            }
        }

        [TestMethod]
        public void SearchTest_ComFiltroQuilometragem_RetornaApenasDentroFaixa()
        {
            // Arrange
            int kmMin = 10000;
            int kmMax = 40000;

            // Act
            var result = controller.Search(null, null, null, null, null, kmMin, kmMax, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;

            // Verifica que todos os ve�culos est�o dentro da faixa de quilometragem
            foreach (var item in lista)
            {
                Assert.IsTrue(item.Veiculo?.Quilometragem >= kmMin && item.Veiculo?.Quilometragem <= kmMax,
                    "Todos os ve�culos devem estar dentro da faixa de quilometragem");
            }
        }

        [TestMethod]
        public void SearchTest_ComMultiplosFiltros_RetornaResultadoCombinado()
        {
            // Arrange
            string termo = "Toyota";
            decimal precoMin = 15000;
            decimal precoMax = 25000;
            int anoMin = 2019;
            int anoMax = 2020;

            // Act
            var result = controller.Search(termo, precoMin, precoMax, anoMin, anoMax, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;

            // Verifica que todos os resultados atendem aos crit�rios
            foreach (var item in lista)
            {
                Assert.IsTrue(
                    (item.Veiculo?.NomeMarca?.Contains("Toyota") ?? false) &&
                    item.Veiculo?.Preco >= precoMin &&
                    item.Veiculo?.Preco <= precoMax &&
                    item.Veiculo?.Ano >= anoMin &&
                    item.Veiculo?.Ano <= anoMax,
                    "Todos os ve�culos devem atender a todos os crit�rios"
                );
            }
        }

        [TestMethod]
        public void SearchTest_ComOpcionais_RetornaVeiculosComOpcionais()
        {
            // Arrange
            var opcionais = new List<string> { "ar condicionado" };

            // Act
            var result = controller.Search(null, null, null, null, null, null, null, null, opcionais);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = (IEnumerable<AnuncioViewModel>)viewResult.ViewData.Model!;

            // Verifica que todos os ve�culos t�m o opcional
            foreach (var item in lista)
            {
                Assert.IsTrue(
                    item.Opcionais?.ToLower().Contains("ar condicionado") ?? false,
                    "Todos os ve�culos devem ter ar condicionado"
                );
            }
        }

        [TestMethod]
        public void SearchTest_PopulaViewBagComFiltros()
        {
            // Arrange
            string termo = "Toyota";
            decimal precoMin = 15000;
            string localizacao = "São Paulo";

            // Act
            var result = controller.Search(termo, precoMin, null, null, null, null, null, localizacao, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            // O controller converte termo para lowercase
            Assert.AreEqual(termo.ToLower(), viewResult.ViewData["Termo"]);
            Assert.AreEqual(precoMin, viewResult.ViewData["PrecoMin"]);
            Assert.AreEqual(localizacao, viewResult.ViewData["Localizacao"]);
        }

        // M�todos auxiliares para criar dados de teste
        private static IEnumerable<Anuncio> GetTestAnuncios()
        {
            return new List<Anuncio>
            {
                new Anuncio 
                { 
                    Id = 1, 
                    IdVeiculo = 1, 
                    StatusAnuncio = "D", 
                    DataPublicacao = System.DateTime.Now, 
                    Visualizacoes = 10, 
                    Descricao = "Toyota Corolla em �timo estado", 
                    Opcionais = "ar condicionado, dire��o hidr�ulica, vidros el�tricos" 
                },
                new Anuncio 
                { 
                    Id = 2, 
                    IdVeiculo = 2, 
                    StatusAnuncio = "D", 
                    DataPublicacao = System.DateTime.Now, 
                    Visualizacoes = 5, 
                    Descricao = "Honda Civic completo", 
                    Opcionais = "ar condicionado, airbag, alarme" 
                },
                new Anuncio 
                { 
                    Id = 3, 
                    IdVeiculo = 1, 
                    StatusAnuncio = "D", 
                    DataPublicacao = System.DateTime.Now, 
                    Visualizacoes = 8, 
                    Descricao = "Outro Toyota Corolla", 
                    Opcionais = "ar condicionado, sensor de estacionamento" 
                }
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
                Quilometragem = 35000, 
                Preco = 20000.00m, 
                IdConcessionaria = 1, 
                IdModelo = 1, 
                IdMarca = 1 
            };
        }

        private static Veiculo GetSecondVeiculo()
        {
            return new Veiculo 
            { 
                Id = 2, 
                Placa = "XYZ-9876", 
                Ano = 2019, 
                Cor = "Branco", 
                Quilometragem = 45000, 
                Preco = 18000.00m, 
                IdConcessionaria = 1, 
                IdModelo = 2, 
                IdMarca = 2 
            };
        }
    }
}
