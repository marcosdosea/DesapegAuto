using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceTests
{
    [TestClass]
    [TestCategory("Integration")]
    public class AdminWorkflowMySqlIntegrationTests
    {
        [TestMethod]
        public void WorkflowAdmin_Marca_PersisteNoMySql()
        {
            var connectionString = LoadConnectionString();
            var suffix = Guid.NewGuid().ToString("N")[..8];
            var nomeMarca = $"IT_MarcaOnly_{suffix}";
            var marcaId = 0;

            try
            {
                using var context = CreateContext(connectionString);
                var marcaService = new MarcaService(context);

                marcaId = marcaService.Create(new Marca { Nome = nomeMarca });

                var marcaSalva = marcaService.Get(marcaId);
                Assert.IsNotNull(marcaSalva);
                Assert.AreEqual(nomeMarca, marcaSalva.Nome);
            }
            finally
            {
                Cleanup(connectionString, 0, 0, marcaId);
            }
        }

        [TestMethod]
        public void WorkflowAdmin_Categoria_PersisteNoMySql()
        {
            var connectionString = LoadConnectionString();
            var suffix = Guid.NewGuid().ToString("N")[..8];
            var nomeCategoria = $"IT_CategoriaOnly_{suffix}";
            var categoriaId = 0;

            try
            {
                using var context = CreateContext(connectionString);
                var categoriaService = new CategoriaService(context);

                categoriaId = categoriaService.Create(new Categoria { Nome = nomeCategoria });

                var categoriaSalva = categoriaService.Get(categoriaId);
                Assert.IsNotNull(categoriaSalva);
                Assert.AreEqual(nomeCategoria, categoriaSalva.Nome);
            }
            finally
            {
                Cleanup(connectionString, 0, categoriaId, 0);
            }
        }

        [TestMethod]
        public void WorkflowAdmin_MarcaCategoriaModelo_PersistemNoMySql()
        {
            var connectionString = LoadConnectionString();
            var suffix = Guid.NewGuid().ToString("N")[..8];

            int marcaId = 0;
            int categoriaId = 0;
            int modeloId = 0;

            try
            {
                using var context = CreateContext(connectionString);
                var marcaService = new MarcaService(context);
                var categoriaService = new CategoriaService(context);
                var modeloService = new ModeloService(context);

                marcaId = marcaService.Create(new Marca { Nome = $"IT_Marca_{suffix}" });
                categoriaId = categoriaService.Create(new Categoria { Nome = $"IT_Categoria_{suffix}" });

                modeloId = modeloService.Create(new Modelo
                {
                    Nome = $"IT_Modelo_{suffix}",
                    IdMarca = marcaId,
                    IdCategoria = categoriaId,
                    Versoes = "Base"
                });

                var modeloSalvo = modeloService.Get(modeloId);
                Assert.IsNotNull(modeloSalvo);
                Assert.AreEqual(marcaId, modeloSalvo.IdMarca);
                Assert.AreEqual(categoriaId, modeloSalvo.IdCategoria);
                Assert.AreEqual($"IT_Categoria_{suffix}", modeloSalvo.Categoria);

                var modelos = modeloService.GetAll().ToList();
                Assert.IsTrue(modelos.Any(m => m.Id == modeloId));

                var modelosDaMarca = modeloService.GetByMarca(marcaId).ToList();
                Assert.IsTrue(modelosDaMarca.Any(m => m.Id == modeloId));
            }
            finally
            {
                Cleanup(connectionString, modeloId, categoriaId, marcaId);
            }
        }

        [TestMethod]
        public void WorkflowAdmin_ListagemModelos_NaoUsaColunaFantasma()
        {
            var connectionString = LoadConnectionString();

            using var context = CreateContext(connectionString);
            var modeloService = new ModeloService(context);

            var modelos = modeloService.GetAll().Take(5).ToList();
            Assert.IsNotNull(modelos);
        }

        private static DesapegAutoContext CreateContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<DesapegAutoContext>()
                .UseMySQL(connectionString)
                .Options;
            return new DesapegAutoContext(options);
        }

        private static void Cleanup(string connectionString, int modeloId, int categoriaId, int marcaId)
        {
            using var context = CreateContext(connectionString);

            if (modeloId > 0)
            {
                context.Database.ExecuteSqlRaw("DELETE FROM modelo WHERE id = {0}", modeloId);
            }

            if (categoriaId > 0)
            {
                context.Database.ExecuteSqlRaw("DELETE FROM categoria WHERE id = {0}", categoriaId);
            }

            if (marcaId > 0)
            {
                context.Database.ExecuteSqlRaw("DELETE FROM marca WHERE id = {0}", marcaId);
            }
        }

        private static string LoadConnectionString()
        {
            var appSettingsPath = LocateAppSettings();
            if (string.IsNullOrWhiteSpace(appSettingsPath) || !File.Exists(appSettingsPath))
            {
                Assert.Inconclusive("Arquivo appsettings.json nao encontrado para testes de integracao MySQL.");
            }

            using var stream = File.OpenRead(appSettingsPath);
            using var document = JsonDocument.Parse(stream);

            JsonElement connValue;
            if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connSection) ||
                !connSection.TryGetProperty("DesapegAutoConnection", out connValue))
            {
                Assert.Inconclusive("ConnectionStrings:DesapegAutoConnection nao encontrada no appsettings.json.");
                return string.Empty;
            }

            var connectionString = connValue.GetString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Assert.Inconclusive("Connection string vazia no appsettings.json.");
            }

            return connectionString!;
        }

        private static string LocateAppSettings()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, "Codigo", "DesapegAutoWeb", "appsettings.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }

            return string.Empty;
        }
    }
}
