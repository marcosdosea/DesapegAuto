using Core;
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
    public class AnuncioServiceTests
    {
        private DesapegAutoContext? _context;
        private IAnuncioService? _anuncioService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _context.Anuncios.AddRange(new List<Anuncio>
            {
                new Anuncio { Id = 1, IdVeiculo = 1, StatusAnuncio = "D", DataPublicacao = DateTime.Now, Visualizacoes = 10, Descricao = "desc1", Opcionais = "opt1" },
                new Anuncio { Id = 2, IdVeiculo = 2, StatusAnuncio = "D", DataPublicacao = DateTime.Now, Visualizacoes = 5, Descricao = "desc2", Opcionais = "opt2" }
            });
            _context.SaveChanges();
            _anuncioService = new AnuncioService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var anuncio = new Anuncio { Id = 3, IdVeiculo = 3, StatusAnuncio = "D", DataPublicacao = DateTime.Now, Visualizacoes = 0, Descricao = "desc3", Opcionais = "opt3" };
            _anuncioService!.Create(anuncio);
            var created = _anuncioService.Get(3);
            Assert.IsNotNull(created);
            Assert.AreEqual("desc3", created.Descricao);
        }

        [TestMethod]
        public void EditTest()
        {
            var anuncio = _anuncioService!.Get(1);
            Assert.IsNotNull(anuncio);
            anuncio.Descricao = "editado";
            _anuncioService.Edit(anuncio);
            var edited = _anuncioService.Get(1);
            Assert.IsNotNull(edited);
            Assert.AreEqual("editado", edited.Descricao);
        }

        [TestMethod]
        public void DeleteTest()
        {
            _anuncioService!.Delete(2);
            var deleted = _anuncioService.Get(2);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public void GetTest()
        {
            var anuncio = _anuncioService!.Get(1);
            Assert.IsNotNull(anuncio);
            Assert.AreEqual(1, anuncio.Id);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var anuncios = _anuncioService!.GetAll();
            Assert.IsNotNull(anuncios);
            Assert.AreEqual(2, anuncios.Count());
        }
    }
}
