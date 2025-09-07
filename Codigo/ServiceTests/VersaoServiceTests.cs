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
    public class VersaoServiceTests
    {
        private DesapegAutoContext context = null!;
        private IVersaoService versaoService = null!;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<DesapegAutoContext>();
            builder.UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString());
            var options = builder.Options;

            context = new DesapegAutoContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var marcas = new List<Marca>
            {
                new() { Id = 1, Nome = "Volkswagen" },
                new() { Id = 2, Nome = "Toyota" }
            };
            context.AddRange(marcas);

            var modelos = new List<Modelo>
            {
                new() { Id = 1, Nome = "Jetta", IdMarca = 1, Categoria = "Sedan", Versoes = "" },
                new() { Id = 2, Nome = "Corolla", IdMarca = 2, Categoria = "Sedan", Versoes = "" }
            };
            context.AddRange(modelos);

            var versoes = new List<Versao>
            {
                new() { Id = 1, Nome = "GLI", IdModelo = 1 },
                new() { Id = 2, Nome = "Comfortline", IdModelo = 1 },
                new() { Id = 3, Nome = "XEi", IdModelo = 2 }
            };
            context.AddRange(versoes);
            context.SaveChanges();

            versaoService = new VersaoService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            versaoService.Create(new Versao()
            {
                Id = 4,
                Nome = "Highline",
                IdModelo = 1
            });

            // Assert
            Assert.AreEqual(4, versaoService.GetAll().Count());
            var versao = versaoService.Get(4);
            Assert.IsNotNull(versao);
            Assert.AreEqual("Highline", versao.Nome);
            Assert.AreEqual(1, versao.IdModelo);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            versaoService.Delete(2);

            // Assert
            Assert.AreEqual(2, versaoService.GetAll().Count());
            var versao = versaoService.Get(2);
            Assert.IsNull(versao);
        }

        [TestMethod()]
        public void Delete_WhenIdDoesNotExist_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => versaoService.Delete(99));
            Assert.IsTrue(exception.Message.Contains("Versão não encontrada"));
        }

        [TestMethod()]
        public void EditTest()
        {
            //Act 
            var versao = versaoService.Get(3);
            Assert.IsNotNull(versao);
            versao.Nome = "Altis";
            versaoService.Edit(versao);

            //Assert
            var versaoEditada = versaoService.Get(3);
            Assert.IsNotNull(versaoEditada);
            Assert.AreEqual("Altis", versaoEditada.Nome);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var versao = versaoService.Get(1);

            // Assert
            Assert.IsNotNull(versao);
            Assert.AreEqual("GLI", versao.Nome);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaVersoes = versaoService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaVersoes, typeof(IEnumerable<Versao>));
            Assert.IsNotNull(listaVersoes);
            Assert.AreEqual(3, listaVersoes.Count());
            Assert.AreEqual(1, listaVersoes.First().Id);
            Assert.AreEqual("GLI", listaVersoes.First().Nome);
        }
    }
}