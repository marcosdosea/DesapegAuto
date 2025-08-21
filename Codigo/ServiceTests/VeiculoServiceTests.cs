using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTO;
namespace Service.Tests
{
    [TestClass()]
    public class VeiculoServiceTests
    {
        private static DesapegAutoContext _context;
        private static IVeiculoService _veiculoService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // Configura o DbContext em memória para os testes
            var options = (new DbContextOptionsBuilder<DesapegAutoContext>())
                .UseInMemoryDatabase(databaseName: "DesapegAuto")
                .Options;

            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Adiciona dados de teste
            _context.Veiculos.AddRange(new List<Veiculo>
            {
                new Veiculo { Id = 1, Placa = "ABC-1234", Ano = 2020, Quilometragem = (int)50000.0, Preco = 30000.00m, IdConcessionaria = 1 },
                new Veiculo { Id = 2, Placa = "DEF-5678", Ano = 2021, Quilometragem = (int)25000.0, Preco = 45000.00m, IdConcessionaria = 1 },
                new Veiculo { Id = 3, Placa = "GHI-9012", Ano = 2020, Quilometragem = (int)60000.0, Preco = 28000.00m, IdConcessionaria = 2 }
            });

            _context.SaveChanges();
            _veiculoService = new VeiculoService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            var veiculo = new Veiculo
            {
                Id = 4,
                Placa = "JKL-0123",
                Ano = 2022,
                Quilometragem = (int)10000.0,
                Preco = 55000.00m
            };

            // Act
            _veiculoService.Create(veiculo);

            // Assert
            var veiculoCriado = _veiculoService.Get(4);
            Assert.IsNotNull(veiculoCriado);
            Assert.AreEqual("JKL-0123", veiculoCriado.Placa);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            var veiculo = _veiculoService.Get(1);
            veiculo.Placa = "EDIT-0000";

            // Act
            _veiculoService.Edit(veiculo);

            // Assert
            var veiculoEditado = _veiculoService.Get(1);
            Assert.AreEqual("EDIT-0000", veiculoEditado.Placa);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Arrange
            int veiculosAntes = _veiculoService.GetAll().Count();

            // Act
            _veiculoService.Delete((uint)2);

            // Assert
            int veiculosDepois = _veiculoService.GetAll().Count();
            Assert.AreEqual(veiculosAntes - 1, veiculosDepois);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var veiculo = _veiculoService.Get(1);

            // Assert
            Assert.IsNotNull(veiculo);
            Assert.AreEqual("ABC-1234", veiculo.Placa);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var veiculos = _veiculoService.GetAll();

            // Assert
            Assert.IsNotNull(veiculos);
            Assert.AreEqual(3, veiculos.Count());
        }

        [TestMethod()]
        public void GetByConcessionariaTest()
        {
            // Act
            var veiculos = _veiculoService.GetByConcessionaria(1);

            // Assert
            Assert.IsNotNull(veiculos);
            Assert.AreEqual(2, veiculos.Count());
        }

        [TestMethod()]
        public void GetByAnoTest()
        {
            // Act
            var veiculos = _veiculoService.GetByAno(2020);

            // Assert
            Assert.IsNotNull(veiculos);
            Assert.AreEqual(2, veiculos.Count());
        }

        [TestMethod()]
        public void GetByQuilometragemTest()
        {
            // Act
            var veiculos = _veiculoService.GetByQuilometragem((int)40000, (int)70000);

            // Assert
            Assert.IsNotNull(veiculos);
            Assert.AreEqual(2, veiculos.Count());
        }

        [TestMethod()]
        public void GetByPrecoTest()
        {
            // Act
            var veiculos = _veiculoService.GetByPreco(25000, 35000);

            // Assert
            Assert.IsNotNull(veiculos);
            Assert.AreEqual(2, veiculos.Count());
        }

        [TestMethod()]
        public void GetByPlacaTest()
        {
            // Act
            var veiculo = _veiculoService.GetByPlaca("ABC-1234");

            // Assert
            Assert.IsNotNull(veiculo);
        }
    }
}