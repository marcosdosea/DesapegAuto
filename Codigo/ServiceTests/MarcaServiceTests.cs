using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Collections.Generic;
using System.Linq;

namespace Service.Tests
{
    [TestClass()]
    public class MarcaServiceTests
    {
        private DesapegAutoContext? _context;
        private IMarcaService? _marcaService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Marcas.AddRange(new List<Marca>
            {
                new Marca { Id = 1, Nome = "Ford" },
                new Marca { Id = 2, Nome = "Chevrolet" },
                new Marca { Id = 3, Nome = "Toyota" }
            });

            _context.SaveChanges();
            _marcaService = new MarcaService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange: Cria uma nova marca
            var novaMarca = new Marca
            {
                Id = 4,
                Nome = "Honda"
            };

            _marcaService!.Create(novaMarca);

            var marcaCriada = _marcaService.Get(4);
            Assert.IsNotNull(marcaCriada);
            Assert.AreEqual("Honda", marcaCriada.Nome);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Arrange: Obtém uma marca existente e modifica seu nome
            var marca = _marcaService!.Get(1);
            Assert.IsNotNull(marca, "Marca não encontrada para edição.");
            marca!.Nome = "Ford Editado";

            // Act: Chama o método Edit do serviço
            _marcaService.Edit(marca);

            // Assert: Verifica se a alteração foi salva
            var marcaEditada = _marcaService.Get(1);
            Assert.IsNotNull(marcaEditada);
            Assert.AreEqual("Ford Editado", marcaEditada.Nome);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Arrange: Armazena a contagem de marcas antes da exclusão
            int marcasAntes = _marcaService!.GetAll().Count();

            // Act: Chama o método Delete do serviço
            _marcaService.Delete(2);

            // Assert: Verifica se a contagem diminuiu em 1
            int marcasDepois = _marcaService.GetAll().Count();
            Assert.AreEqual(marcasAntes - 1, marcasDepois);
            var marcaDeletada = _marcaService.Get(2);
            Assert.IsNull(marcaDeletada);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act: Busca uma marca específica pelo ID
            var marca = _marcaService!.Get(1);

            // Assert: Verifica se a marca correta foi retornada
            Assert.IsNotNull(marca);
            Assert.AreEqual("Ford", marca.Nome);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act: Busca todas as marcas
            var marcas = _marcaService!.GetAll();

            // Assert: Verifica se a quantidade de marcas está correta
            Assert.IsNotNull(marcas);
            Assert.AreEqual(3, marcas.Count());
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            // Act: Busca marcas por um nome específico
            var marcas = _marcaService!.GetByNome("Chevrolet");

            // Assert: Verifica se a busca retornou os resultados esperados
            Assert.IsNotNull(marcas);
            Assert.AreEqual(1, marcas.Count());
            Assert.AreEqual("Chevrolet", marcas.First().Nome);
        }

        [TestMethod()]
        public void Create_QuandoNomeVazio_DeveLancarServiceException()
        {
            var exception = Assert.ThrowsException<ServiceException>(() =>
                _marcaService!.Create(new Marca { Id = 4, Nome = "   " }));

            Assert.IsTrue(exception.Message.Contains("Nome da marca"));
        }

        [TestMethod()]
        public void Create_QuandoNomeDuplicado_DeveLancarServiceException()
        {
            var exception = Assert.ThrowsException<ServiceException>(() =>
                _marcaService!.Create(new Marca { Id = 4, Nome = "ford" }));

            Assert.IsTrue(exception.Message.Contains("Marca ja existente"));
        }
    }
}