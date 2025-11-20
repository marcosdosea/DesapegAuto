using Core;
using Microsoft.EntityFrameworkCore;

namespace DesapegAutoWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DesapegAutoContext context)
        {
            // Ensure database exists before attempting to seed baseline data
            context.Database.EnsureCreated();

            // Look for any vehicles.
            if (context.Veiculos.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Concessionaria
            var concessionaria = new Concessionaria
            {
                Nome = "DesapegAuto Matriz",
                Cnpj = "12345678000199",
                Email = "contato@desapegauto.com",
                Telefone = "11999999999",
                Endereco = "Av. Paulista, 1000",
                Senha = "admin"
            };
            context.Concessionaria.Add(concessionaria);
            context.SaveChanges();

            // Seed Marcas
            var marcas = new Marca[]
            {
                new Marca { Nome = "Toyota" },
                new Marca { Nome = "Honda" },
                new Marca { Nome = "Ford" },
                new Marca { Nome = "BMW" },
                new Marca { Nome = "Porsche" }
            };
            context.Marcas.AddRange(marcas);
            context.SaveChanges();

            // Seed Modelos
            var modelos = new Modelo[]
            {
                new Modelo { Nome = "Corolla", IdMarca = marcas[0].Id, Categoria = "Sedan", Versoes = "XEi, Altis" },
                new Modelo { Nome = "Civic", IdMarca = marcas[1].Id, Categoria = "Sedan", Versoes = "Touring, Si" },
                new Modelo { Nome = "Mustang", IdMarca = marcas[2].Id, Categoria = "Esportivo", Versoes = "GT, Mach 1" },
                new Modelo { Nome = "X5", IdMarca = marcas[3].Id, Categoria = "SUV", Versoes = "xDrive45e" },
                new Modelo { Nome = "911", IdMarca = marcas[4].Id, Categoria = "Esportivo", Versoes = "Carrera, Turbo S" }
            };
            context.Modelos.AddRange(modelos);
            context.SaveChanges();

            // Seed Veiculos
            var veiculos = new Veiculo[]
            {
                new Veiculo { IdConcessionaria = concessionaria.Id, IdMarca = marcas[0].Id, IdModelo = modelos[0].Id, Ano = 2023, Cor = "Branco", Quilometragem = 15000, Preco = 140000, Placa = "ABC1234" },
                new Veiculo { IdConcessionaria = concessionaria.Id, IdMarca = marcas[1].Id, IdModelo = modelos[1].Id, Ano = 2022, Cor = "Prata", Quilometragem = 25000, Preco = 130000, Placa = "DEF5678" },
                new Veiculo { IdConcessionaria = concessionaria.Id, IdMarca = marcas[2].Id, IdModelo = modelos[2].Id, Ano = 2024, Cor = "Vermelho", Quilometragem = 1000, Preco = 450000, Placa = "GHI9012" },
                new Veiculo { IdConcessionaria = concessionaria.Id, IdMarca = marcas[3].Id, IdModelo = modelos[3].Id, Ano = 2023, Cor = "Preto", Quilometragem = 5000, Preco = 600000, Placa = "JKL3456" },
                new Veiculo { IdConcessionaria = concessionaria.Id, IdMarca = marcas[4].Id, IdModelo = modelos[4].Id, Ano = 2021, Cor = "Amarelo", Quilometragem = 8000, Preco = 950000, Placa = "MNO7890" }
            };
            context.Veiculos.AddRange(veiculos);
            context.SaveChanges();

            // Seed Anuncios
            var anuncios = new Anuncio[]
            {
                new Anuncio { IdVeiculo = veiculos[0].Id, DataPublicacao = DateTime.Now, StatusAnuncio = "Ativo", Visualizacoes = 10, Descricao = "Corolla impecável", Opcionais = "Ar condicionado, Vidro elétrico", IdVenda = 0 },
                new Anuncio { IdVeiculo = veiculos[1].Id, DataPublicacao = DateTime.Now, StatusAnuncio = "Ativo", Visualizacoes = 25, Descricao = "Civic completo", Opcionais = "Teto solar, Couro", IdVenda = 0 },
                new Anuncio { IdVeiculo = veiculos[2].Id, DataPublicacao = DateTime.Now, StatusAnuncio = "Ativo", Visualizacoes = 150, Descricao = "Mustang V8", Opcionais = "Sport, Automático", IdVenda = 0 },
                new Anuncio { IdVeiculo = veiculos[3].Id, DataPublicacao = DateTime.Now, StatusAnuncio = "Ativo", Visualizacoes = 80, Descricao = "BMW X5 Híbrida", Opcionais = "4x4, Híbrido", IdVenda = 0 },
                new Anuncio { IdVeiculo = veiculos[4].Id, DataPublicacao = DateTime.Now, StatusAnuncio = "Ativo", Visualizacoes = 300, Descricao = "Porsche 911 Carrera", Opcionais = "Sport, Couro", IdVenda = 0 }
            };
            context.Anuncios.AddRange(anuncios);
            context.SaveChanges();
        }
    }
}
