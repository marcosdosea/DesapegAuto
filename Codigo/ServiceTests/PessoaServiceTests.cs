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
    public class PessoaServiceTests
    {
        private DesapegAutoContext? _context;
        private IPessoaService? _pessoaService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new DesapegAutoContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Pessoas.AddRange(new List<Pessoa>
            {
                new Pessoa { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888" },
                new Pessoa { Id = 2, Nome = "Maria Oliveira", Cpf = "55566677788", Email = "maria@email.com", Telefone = "11988887777" },
                new Pessoa { Id = 3, Nome = "Carlos Pereira", Cpf = "99988877766", Email = "carlos@email.com", Telefone = "11777766655" }
            });
            _context.SaveChanges();

            _pessoaService = new PessoaService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            var nova = new Pessoa { Id = 4, Nome = "Ana", Cpf = "22233344455", Email = "ana@email.com", Telefone = "11911112222" };
            _pessoaService!.Create(nova);

            var todas = _pessoaService.GetAll();
            Assert.AreEqual(4, todas.Count());
            var encontrada = _pessoaService.Get(4);
            Assert.IsNotNull(encontrada);
            Assert.AreEqual("Ana", encontrada!.Nome);
        }

        [TestMethod()]
        public void EditTest()
        {
            var pessoa = _pessoaService!.Get(1);
            Assert.IsNotNull(pessoa);
            pessoa!.Nome = "João Editado";
            _pessoaService.Edit(pessoa);

            var editada = _pessoaService.Get(1);
            Assert.IsNotNull(editada);
            Assert.AreEqual("João Editado", editada!.Nome);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            _pessoaService!.Delete(2);
            var todas = _pessoaService.GetAll();
            Assert.AreEqual(2, todas.Count());
            var excluida = _pessoaService.Get(2);
            Assert.IsNull(excluida);
        }

        [TestMethod()]
        public void GetTest()
        {
            var pessoa = _pessoaService!.Get(1);
            Assert.IsNotNull(pessoa);
            Assert.AreEqual("João Silva", pessoa!.Nome);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            var todas = _pessoaService!.GetAll();
            Assert.IsNotNull(todas);
            Assert.AreEqual(3, todas.Count());
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            var resultado = _pessoaService!.GetByNome("Maria");
            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count());
            Assert.AreEqual("Maria Oliveira", resultado.First().Nome);
        }

        [TestMethod()]
        public void CreateTest_CpfDuplicado_ThrowsServiceException()
        {
            // Arrange - Tentando criar pessoa com CPF já existente
            var pessoaCpfDuplicado = new Pessoa
            {
                Id = 4,
                Nome = "Novo Usuario",
                Cpf = "11122233344", // CPF já existe (João Silva)
                Email = "novo@email.com",
                Telefone = "11900001111"
            };

            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _pessoaService!.Create(pessoaCpfDuplicado));
            Assert.IsTrue(exception.Message.Contains("CPF já cadastrado"));
        }

        [TestMethod()]
        public void EditTest_PessoaNaoEncontrada_ThrowsServiceException()
        {
            // Arrange - Pessoa com ID inexistente
            var pessoaInexistente = new Pessoa
            {
                Id = 99,
                Nome = "Pessoa Inexistente",
                Cpf = "00000000000",
                Email = "inexistente@email.com",
                Telefone = "11900000000"
            };

            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _pessoaService!.Edit(pessoaInexistente));
            Assert.IsTrue(exception.Message.Contains("Pessoa não encontrada"));
        }

        [TestMethod()]
        public void DeleteTest_PessoaNaoEncontrada_ThrowsServiceException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => _pessoaService!.Delete(99));
            Assert.IsTrue(exception.Message.Contains("Pessoa não encontrada"));
        }
    }
}
