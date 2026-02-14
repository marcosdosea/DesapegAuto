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
    public class CategoriaServiceTests
    {
        private DesapegAutoContext? _context;
        private ICategoriaService? _categoriaService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Categoria.AddRange(new List<Categoria>
            {
                new Categoria { Id = 1, Nome = "SUV" },
                new Categoria { Id = 2, Nome = "Sedan" },
                new Categoria { Id = 3, Nome = "Hatch" }
            });

            _context.SaveChanges();
            _categoriaService = new CategoriaService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            var novaCategoria = new Categoria
            {
                Id = 4,
                Nome = "Convertible"
            };

            _categoriaService!.Create(novaCategoria);

            var categoriaCriada = _categoriaService.Get(4);
            Assert.IsNotNull(categoriaCriada);
            Assert.AreEqual("Convertible", categoriaCriada.Nome);
        }

        [TestMethod()]
        public void EditTest()
        {
            var categoria = _categoriaService!.Get(1);
            Assert.IsNotNull(categoria);
            categoria!.Nome = "SUV Editado";

            _categoriaService.Edit(categoria);

            var categoriaEditada = _categoriaService.Get(1);
            Assert.IsNotNull(categoriaEditada);
            Assert.AreEqual("SUV Editado", categoriaEditada.Nome);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            int categoriasAntes = _categoriaService!.GetAll().Count();

            _categoriaService.Delete(2);

            int categoriasDepois = _categoriaService.GetAll().Count();
            Assert.AreEqual(categoriasAntes - 1, categoriasDepois);
            var categoriaDeletada = _categoriaService.Get(2);
            Assert.IsNull(categoriaDeletada);
        }

        [TestMethod()]
        public void GetTest()
        {
            var categoria = _categoriaService!.Get(1);
            Assert.IsNotNull(categoria);
            Assert.AreEqual("SUV", categoria.Nome);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            var categorias = _categoriaService!.GetAll();
            Assert.IsNotNull(categorias);
            Assert.AreEqual(3, categorias.Count());
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            var categorias = _categoriaService!.GetByNome("Sedan");
            Assert.IsNotNull(categorias);
            Assert.AreEqual(1, categorias.Count());
            Assert.AreEqual("Sedan", categorias.First().Nome);
        }

        [TestMethod()]
        public void Create_QuandoNomeVazio_DeveLancarServiceException()
        {
            var exception = Assert.ThrowsException<ServiceException>(() =>
                _categoriaService!.Create(new Categoria { Id = 4, Nome = "   " }));

            Assert.IsTrue(exception.Message.Contains("Nome da categoria"));
        }

        [TestMethod()]
        public void Create_QuandoNomeDuplicado_DeveLancarServiceException()
        {
            var exception = Assert.ThrowsException<ServiceException>(() =>
                _categoriaService!.Create(new Categoria { Id = 4, Nome = "suv" }));

            Assert.IsTrue(exception.Message.Contains("Categoria ja existente"));
        }
    }
}