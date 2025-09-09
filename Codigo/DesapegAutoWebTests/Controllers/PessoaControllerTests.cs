using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Controllers;
using DesapegAutoWeb.Mappers;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DesapegAutoWebTests.Controllers
{
    [TestClass()]
    public class PessoaControllerTests
    {
        private static PessoaController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            var mockPessoaService = new Mock<IPessoaService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PessoaProfile())).CreateMapper();

            mockPessoaService.Setup(service => service.GetAll())
                .Returns(GetTestPessoas());
            mockPessoaService.Setup(service => service.Get(1))
                .Returns(GetTargetPessoa());
            mockPessoaService.Setup(service => service.Create(It.IsAny<Pessoa>()))
                .Returns(4);
            mockPessoaService.Setup(service => service.Edit(It.IsAny<Pessoa>()))
                .Verifiable();
            mockPessoaService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            controller = new PessoaController(mockPessoaService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            var result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<PessoaViewModel>));
            var lista = (List<PessoaViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PessoaViewModel));
            var model = (PessoaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("João Silva", model.Nome);
        }

        [TestMethod()]
        public void CreateTest_Post_Valido()
        {
            var vm = new PessoaViewModel { Id = 4, Nome = "Ana", Cpf = "22233344455", Email = "ana@email.com", Telefone = "11911112222", Senha = "senha1234", ConfirmacaoSenha = "senha1234" };
            var result = controller.Create(vm);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            var vm = new PessoaViewModel { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888", Senha = "senha1234", ConfirmacaoSenha = "senha1234" };
            var result = controller.Edit(1, vm);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valido()
        {
            var vm = new PessoaViewModel { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888", Senha = "senha1234", ConfirmacaoSenha = "senha1234" };
            var result = controller.Delete(1, vm);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        private static IEnumerable<Pessoa> GetTestPessoas()
        {
            return new List<Pessoa>
            {
                new Pessoa { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888" },
                new Pessoa { Id = 2, Nome = "Maria Oliveira", Cpf = "55566677788", Email = "maria@email.com", Telefone = "11988887777" },
                new Pessoa { Id = 3, Nome = "Carlos Pereira", Cpf = "99988877766", Email = "carlos@email.com", Telefone = "11777766655" }
            };
        }

        private static Pessoa GetTargetPessoa()
        {
            return new Pessoa { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888" };
        }
    }
}
