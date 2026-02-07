using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;

namespace ServiceTests
{
    [TestClass]
    public class ConcessionariaServiceTests
    {
        private DesapegAutoContext context = null!;
        private IConcessionariaService concessionariaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DesapegAutoContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            concessionariaService = new ConcessionariaService(context);
        }

        [TestMethod]
        public void Create_ComCnpjSomenteDigitos_FormataAntesDeSalvar()
        {
            var concessionaria = new Concessionaria
            {
                Nome = "Auto Prime",
                Cnpj = "12345678000190",
                Email = "contato@autoprime.com",
                Telefone = "11999999999",
                Senha = "12345678",
                Endereco = "Rua 1"
            };

            var id = concessionariaService.Create(concessionaria);
            var saved = concessionariaService.Get(id);

            Assert.IsNotNull(saved);
            Assert.AreEqual("12.345.678/0001-90", saved.Cnpj);
        }

        [TestMethod]
        public void Create_ComCnpjInvalido_LancaServiceException()
        {
            var concessionaria = new Concessionaria
            {
                Nome = "Auto Prime",
                Cnpj = "123",
                Email = "contato@autoprime.com",
                Telefone = "11999999999",
                Senha = "12345678",
                Endereco = "Rua 1"
            };

            Assert.ThrowsException<ServiceException>(() => concessionariaService.Create(concessionaria));
        }
    }
}
