using Core;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass]
    public class CompraControllerTests
    {
        private static CompraController controller = null!;
        private static Mock<IAnuncioService> mockAnuncioService = null!;
        private static Mock<IVeiculoService> mockVeiculoService = null!;
        private static Mock<IMarcaService> mockMarcaService = null!;
        private static Mock<IModeloService> mockModeloService = null!;
        private static Mock<IConcessionariaService> mockConcessionariaService = null!;
        private static Mock<IPessoaService> mockPessoaService = null!;
        private static Mock<IVendaService> mockVendaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockAnuncioService = new Mock<IAnuncioService>();
            mockVeiculoService = new Mock<IVeiculoService>();
            mockMarcaService = new Mock<IMarcaService>();
            mockModeloService = new Mock<IModeloService>();
            mockConcessionariaService = new Mock<IConcessionariaService>();
            mockPessoaService = new Mock<IPessoaService>();
            mockVendaService = new Mock<IVendaService>();

            mockAnuncioService.Setup(s => s.Get(1)).Returns(new Anuncio
            {
                Id = 1,
                IdVeiculo = 10,
                IdVenda = 0,
                StatusAnuncio = "D",
                Descricao = "desc",
                Opcionais = "opt"
            });
            mockAnuncioService.Setup(s => s.Get(2)).Returns(new Anuncio
            {
                Id = 2,
                IdVeiculo = 10,
                IdVenda = 99,
                StatusAnuncio = "V",
                Descricao = "vendido",
                Opcionais = "opt"
            });

            mockVeiculoService.Setup(s => s.Get(10)).Returns(new Veiculo
            {
                Id = 10,
                IdConcessionaria = 3,
                IdMarca = 4,
                IdModelo = 5,
                Ano = 2022,
                Cor = "Preto",
                Quilometragem = 12000,
                Preco = 85000.00m,
                Placa = "ABC-1234"
            });

            mockMarcaService.Setup(s => s.Get(4)).Returns(new Marca { Id = 4, Nome = "Toyota" });
            mockModeloService.Setup(s => s.Get(5)).Returns(new Modelo { Id = 5, Nome = "Corolla", IdMarca = 4 });
            mockConcessionariaService.Setup(s => s.Get(3)).Returns(new Concessionaria
            {
                Id = 3,
                Nome = "Concessionaria Teste",
                Cnpj = "00.000.000/0001-00",
                Email = "contato@teste.com",
                Endereco = "Rua A",
                Telefone = "11999999999",
                Senha = "abc123"
            });

            mockPessoaService.Setup(s => s.Create(It.IsAny<Pessoa>()))
                .Callback<Pessoa>(p => p.Id = 11)
                .Returns(11);
            mockVendaService.Setup(s => s.Create(It.IsAny<Venda>()))
                .Callback<Venda>(v => v.Id = 21)
                .Returns(21);

            controller = new CompraController(
                mockAnuncioService.Object,
                mockVeiculoService.Object,
                mockMarcaService.Object,
                mockModeloService.Object,
                mockConcessionariaService.Object,
                mockPessoaService.Object,
                mockVendaService.Object);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [TestMethod]
        public void CreateGet_QuandoAnuncioVendido_DeveRedirecionarParaHomeIndex()
        {
            var result = controller.Create(2);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);
            Assert.AreEqual("Este veiculo ja foi vendido.", controller.TempData["ErrorMessage"]);
        }

        [TestMethod]
        public void CreatePost_QuandoPessoaNaoExiste_DeveCriarPessoaVendaEAtualizarAnuncio()
        {
            mockPessoaService.Setup(s => s.GetByCpf(It.IsAny<string>())).Returns((Pessoa?)null);
            mockPessoaService.Setup(s => s.GetByEmail(It.IsAny<string>())).Returns((Pessoa?)null);

            var model = new CompraViewModel
            {
                IdAnuncio = 1,
                Nome = "Cliente Novo",
                Email = "novo@email.com",
                Cpf = "12345678901",
                Telefone = "11999998888",
                FormaPagamento = "Financiamento",
                ValorFinal = 85000.00m
            };

            var result = controller.Create(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Confirmacao", redirect.ActionName);

            mockPessoaService.Verify(s => s.Create(It.IsAny<Pessoa>()), Times.Once);
            mockVendaService.Verify(s => s.Create(It.Is<Venda>(v =>
                v.IdConcessionaria == 3 &&
                v.IdPessoa == 11 &&
                v.FormaPagamento == "Financiamento")), Times.Once);
            mockAnuncioService.Verify(s => s.Edit(It.Is<Anuncio>(a =>
                a.Id == 1 &&
                a.IdVenda == 21 &&
                a.StatusAnuncio == "P")), Times.Once);
        }

        [TestMethod]
        public void CreatePost_QuandoPessoaExiste_NaoDeveCriarPessoaENaoDuplicarCadastro()
        {
            var pessoaExistente = new Pessoa
            {
                Id = 30,
                Nome = "Cliente Existente",
                Email = "existente@email.com",
                Cpf = "99999999999",
                Telefone = "11911112222"
            };

            mockPessoaService.Setup(s => s.GetByCpf("99999999999")).Returns(pessoaExistente);

            var model = new CompraViewModel
            {
                IdAnuncio = 1,
                Nome = "Cliente Existente",
                Email = "existente@email.com",
                Cpf = "99999999999",
                Telefone = "11911112222",
                FormaPagamento = "Pix",
                ValorFinal = 85000.00m
            };

            var result = controller.Create(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            mockPessoaService.Verify(s => s.Create(It.IsAny<Pessoa>()), Times.Never);
            mockVendaService.Verify(s => s.Create(It.Is<Venda>(v => v.IdPessoa == 30)), Times.Once);
        }
    }
}
