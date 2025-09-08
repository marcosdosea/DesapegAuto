﻿using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Tests
{
    [TestClass()]
    public class VendaServiceTests
    {
        private DesapegAutoContext context = null!;
        private IVendaService vendaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<DesapegAutoContext>();
            builder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            var options = builder.Options;

            context = new DesapegAutoContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var pessoas = new List<Pessoa>
            {
                new() { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@email.com", Telefone = "11999998888" },
                new() { Id = 2, Nome = "Maria Oliveira", Cpf = "55566677788", Email = "maria@email.com", Telefone = "11988887777" }
            };
            context.Pessoas.AddRange(pessoas);

            var concessionarias = new List<Concessionaria>
            {
                new() { Id = 1, Nome = "AutoFácil", Cnpj = "12345678000100", Email = "contato@autofacil.com", Telefone = "1133334444", Senha = "senha123", IdEndereco = 1 },
                new() { Id = 2, Nome = "VendeCar", Cnpj = "98765432000199", Email = "contato@vendecar.com", Telefone = "1144445555", Senha = "senha456", IdEndereco = 2 }
            };
            context.Concessionaria.AddRange(concessionarias);

            var vendas = new List<Venda>
            {
                new() { Id = 1, DataVenda = DateTime.Now, ValorFinal = 50000.00m, FormaPagamento = "Financiamento", IdConcessionaria = 1, IdPessoa = 1 },
                new() { Id = 2, DataVenda = DateTime.Now.AddDays(-10), ValorFinal = 75000.00m, FormaPagamento = "À Vista", IdConcessionaria = 2, IdPessoa = 2 }
            };
            context.Venda.AddRange(vendas);
            context.SaveChanges();

            vendaService = new VendaService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            vendaService.Create(new Venda()
            {
                Id = 3,
                DataVenda = DateTime.Now,
                ValorFinal = 90000.00m,
                FormaPagamento = "Consórcio",
                IdConcessionaria = 1,
                IdPessoa = 2
            });

            // Assert
            Assert.AreEqual(3, vendaService.GetAll().Count());
            var venda = vendaService.Get(3);
            Assert.IsNotNull(venda);
            Assert.AreEqual(90000.00m, venda.ValorFinal);
            Assert.AreEqual(1, venda.IdConcessionaria);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            vendaService.Delete(2);

            // Assert
            Assert.AreEqual(1, vendaService.GetAll().Count());
            var venda = vendaService.Get(2);
            Assert.IsNull(venda);
        }

        [TestMethod()]
        public void Delete_WhenIdDoesNotExist_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ServiceException>(() => vendaService.Delete(99));
            Assert.IsTrue(exception.Message.Contains("Venda não encontrada"));
        }

        [TestMethod()]
        public void EditTest()
        {
            //Act 
            var venda = vendaService.Get(1);
            Assert.IsNotNull(venda);
            venda.ValorFinal = 52500.00m;
            venda.FormaPagamento = "Cartão de Crédito";
            vendaService.Edit(venda);

            //Assert
            var vendaEditada = vendaService.Get(1);
            Assert.IsNotNull(vendaEditada);
            Assert.AreEqual(52500.00m, vendaEditada.ValorFinal);
            Assert.AreEqual("Cartão de Crédito", vendaEditada.FormaPagamento);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var venda = vendaService.Get(1);

            // Assert
            Assert.IsNotNull(venda);
            Assert.AreEqual(50000.00m, venda.ValorFinal);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaVendas = vendaService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaVendas, typeof(IEnumerable<Venda>));
            Assert.IsNotNull(listaVendas);
            Assert.AreEqual(2, listaVendas.Count());
        }

        [TestMethod()]
        public void GetAllDTOTest()
        {
            // Act
            var listaVendasDTO = vendaService.GetAllDTO();
            var primeiraVenda = listaVendasDTO.First();

            // Assert
            Assert.IsNotNull(listaVendasDTO);
            Assert.AreEqual(2, listaVendasDTO.Count());
            Assert.AreEqual("AutoFácil", primeiraVenda.NomeConcessionaria);
            Assert.AreEqual("João Silva", primeiraVenda.NomePessoa);
        }
    }
}