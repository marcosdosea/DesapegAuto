using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceTests
{
    [TestClass]
    public class ModeloServiceTests
    {
        private DesapegAutoContext? _context;
        private IModeloService? _modeloService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Inserir marcas para relacionamento
            var marcas = new List<Marca>
            {
                new Marca { Id = 1, Nome = "Toyota" },
                new Marca { Id = 2, Nome = "Honda" },
                new Marca { Id = 3, Nome = "Volkswagen" }
            };
            _context.Marcas.AddRange(marcas);

            _context.Modelos.AddRange(new List<Modelo>
            {
                new Modelo { Id = 1, Nome = "Corolla", IdMarca = 1, Categoria = "Sedan", Versoes = "GLi,XLi" },
                new Modelo { Id = 2, Nome = "Civic", IdMarca = 2, Categoria = "Sedan", Versoes = "LX,EX" },
                new Modelo { Id = 3, Nome = "Jetta", IdMarca = 3, Categoria = "Sedan", Versoes = "Trendline,Comfortline" }
            });
            _context.SaveChanges();
            _modeloService = new ModeloService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var modelo = new Modelo { Id = 4, Nome = "Onix", IdMarca = 1 };
            _modeloService!.Create(modelo);
            var created = _modeloService.Get(4);
            Assert.IsNotNull(created);
            Assert.AreEqual("Onix", created.Nome);
        }

        [TestMethod]
        public void EditTest()
        {
            var modelo = _modeloService!.Get(1);
            Assert.IsNotNull(modelo);
            modelo.Nome = "Editado";
            _modeloService.Edit(modelo);
            var edited = _modeloService.Get(1);
            Assert.AreEqual("Editado", edited.Nome);
        }

        [TestMethod]
        public void DeleteTest()
        {
            _modeloService!.Delete(2);
            var deleted = _modeloService.Get(2);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public void GetTest()
        {
            var modelo = _modeloService!.Get(1);
            Assert.IsNotNull(modelo);
            Assert.AreEqual(1, modelo.Id);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var modelos = _modeloService!.GetAll();
            Assert.IsNotNull(modelos);
            Assert.AreEqual(3, modelos.Count());
        }

        [TestMethod]
        public void CreateTest_ModeloDuplicadoPorMarca_ThrowsServiceException()
        {
            // Arrange - Tentando criar modelo com mesmo nome e marca já existentes
            var modeloDuplicado = new Modelo
            {
                Id = 5,
                Nome = "Corolla", // Mesmo nome do modelo ID 1
                IdMarca = 1, // Mesma marca do modelo ID 1
                Categoria = "Sedan",
                Versoes = "GLi,XLi"
            };

            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _modeloService!.Create(modeloDuplicado));
            Assert.IsTrue(exception.Message.Contains("Modelo já existente"));
        }

        [TestMethod]
        public void GetByMarca_VerificaIntegridadeDosDadosRelacionados()
        {
            // Arrange - Buscar todos os modelos da marca Toyota (ID = 1)
            
            // Act
            var modelosToyota = _modeloService!.GetByMarca(1).ToList();

            // Assert
            Assert.IsNotNull(modelosToyota);
            Assert.AreEqual(1, modelosToyota.Count());
            var corolla = modelosToyota.First();
            Assert.AreEqual("Corolla", corolla.Nome);
            Assert.AreEqual(1, corolla.IdMarca);
        }

        [TestMethod]
        public void GetByNome_BuscaComSucessoePorPadrãoCorreto()
        {
            // Arrange - Buscar modelo por nome parcial
            
            // Act
            var modelos = _modeloService!.GetByNome("Civ").ToList();

            // Assert
            Assert.IsNotNull(modelos);
            Assert.IsTrue(modelos.Count() > 0);
            var civic = modelos.FirstOrDefault(m => m.Nome == "Civic");
            Assert.IsNotNull(civic);
            Assert.AreEqual(2, civic.Id);
            Assert.AreEqual(2, civic.IdMarca);
        }

        [TestMethod]
        public void EditTest_ModeloNaoEncontrado_ThrowsServiceException()
        {
            // Arrange - Modelo com ID inexistente
            var modeloInexistente = new Modelo
            {
                Id = 99,
                Nome = "ModeloFantasma",
                IdMarca = 1,
                Categoria = "SUV"
            };

            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _modeloService!.Edit(modeloInexistente));
            Assert.IsTrue(exception.Message.Contains("Modelo não encontrado"));
        }

        [TestMethod]
        public void DeleteTest_ModeloNaoEncontrado_ThrowsServiceException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _modeloService!.Delete(99));
            Assert.IsTrue(exception.Message.Contains("Modelo não encontrado"));
        }
    }
}
