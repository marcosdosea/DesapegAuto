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
            _context.Modelos.AddRange(new List<Modelo>
            {
                new Modelo { Id = 1, Nome = "Corolla" },
                new Modelo { Id = 2, Nome = "Civic" }
            });
            _context.SaveChanges();
            _modeloService = new ModeloService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var modelo = new Modelo { Id = 3, Nome = "Onix" };
            _modeloService!.Create(modelo);
            var created = _modeloService.Get(3);
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
            Assert.AreEqual(2, modelos.Count());
        }
    }
}
