using Core;
using Core.DTO;
using Core.Service;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class VeiculoService : IVeiculoService
    {
        private readonly DesapegAutoContext _context;

        public VeiculoService(DesapegAutoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cadastrar um novo veículo na base de dados
        /// </summary>
        /// <param name="veiculo">Dados do veículo</param>
        /// <returns>ID do veículo</returns>
        public int Create(Veiculo veiculo)
        {
            var veiculoExistente = _context.Veiculos
                .FirstOrDefault(v => v.Placa.ToLower() == veiculo.Placa.ToLower());

            if (veiculoExistente != null)
            {
                throw new ServiceException("Erro: Já existe um veículo cadastrado com esta placa.");
            }

            _context.Add(veiculo);
            _context.SaveChanges();
            return veiculo.Id;
        }

        /// <summary>
        /// Editar dados de um veículo na base de dados
        /// </summary>
        /// <param name="veiculo">Dados do veículo</param>
        public void Edit(Veiculo veiculo)
        {
            var veiculoExistente = _context.Veiculos.Find(veiculo.Id);
            if (veiculoExistente == null)
            {
                throw new ServiceException("Erro: Veículo não encontrado. A operação foi cancelada.");
            }

            var veiculoMesmaPlaca = _context.Veiculos
                .FirstOrDefault(v => v.Id != veiculo.Id &&
                                   v.Placa.ToLower() == veiculo.Placa.ToLower());
            if (veiculoMesmaPlaca != null)
            {
                throw new ServiceException("Erro: Já existe outro veículo cadastrado com esta placa.");
            }

            veiculoExistente.Placa = veiculo.Placa;
            veiculoExistente.Ano = veiculo.Ano;
            veiculoExistente.Cor = veiculo.Cor;
            veiculoExistente.Quilometragem = veiculo.Quilometragem;
            veiculoExistente.Preco = veiculo.Preco;
            veiculoExistente.IdConcessionaria = veiculo.IdConcessionaria;
            veiculoExistente.IdModelo = veiculo.IdModelo;
            veiculoExistente.IdMarca = veiculo.IdMarca;
            _context.SaveChanges();
        }

        /// <summary>
        /// Remover um veículo da base de dados
        /// </summary>
        /// <param name="id">ID do veículo</param>
        public void Delete(int id)
        {
            var veiculo = _context.Veiculos.Find(id);
            if (veiculo == null)
            {
                throw new ServiceException("Erro: Veículo não encontrado. A operação foi cancelada.");
            }

            _context.Remove(veiculo);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obter dados de um veículo
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Dados do veículo</returns>
        public Veiculo? Get(int id)
        {
            return _context.Veiculos.Find(id);
        }

        /// <summary>
        /// Obter todos os veículos cadastrados
        /// </summary>
        /// <returns>Lista de todos os veículos</returns>
        public IEnumerable<Veiculo> GetAll()
        {
            return _context.Veiculos.AsNoTracking();
        }

        public IEnumerable<VeiculoDTO> GetByConcessionaria(int idConcessionaria)
        {
            return _context.Veiculos
                .Where(v => v.IdConcessionaria == idConcessionaria)
                .Select(v => new VeiculoDTO
                {
                    Id = v.Id,
                    Concessionaria = v.IdConcessionaria,
                    Ano = v.Ano,
                    Cor = v.Cor,
                    Quilometragem = v.Quilometragem,
                    Preco = v.Preco,
                    Placa = v.Placa
                }).ToList();
        }

        public IEnumerable<VeiculoDTO> GetByAno(int ano)
        {
            return _context.Veiculos
                .Where(v => v.Ano == ano)
                .Select(v => new VeiculoDTO
                {
                    Id = v.Id,
                    Concessionaria = v.IdConcessionaria,
                    Ano = v.Ano,
                    Cor = v.Cor,
                    Quilometragem = v.Quilometragem,
                    Preco = v.Preco,
                    Placa = v.Placa
                }).ToList();
        }

        public IEnumerable<VeiculoDTO> GetByQuilometragem(int quilometragem, int v)
        {
            return _context.Veiculos
                .Where(veiculo => veiculo.Quilometragem >= quilometragem && veiculo.Quilometragem <= v)
                .Select(veiculo => new VeiculoDTO
                {
                    Id = veiculo.Id,
                    Concessionaria = veiculo.IdConcessionaria,
                    Ano = veiculo.Ano,
                    Cor = veiculo.Cor,
                    Quilometragem = veiculo.Quilometragem,
                    Preco = veiculo.Preco,
                    Placa = veiculo.Placa
                })
                .ToList();
        }

        public IEnumerable<VeiculoDTO> GetByPreco(decimal precoMin, int precoMax)
        {
            return _context.Veiculos
                .Where(v => v.Preco >= precoMin && v.Preco <= precoMax)
                .Select(v => new VeiculoDTO
                {
                    Id = v.Id,
                    Placa = v.Placa,
                    Ano = v.Ano,
                    Quilometragem = v.Quilometragem,
                    Preco = v.Preco,
                    Cor = v.Cor
                })
                .ToList();
        }

        public IEnumerable<VeiculoDTO> GetByPlaca(string placa)
        {
            return _context.Veiculos
                .Where(v => v.Placa.Contains(placa))
                .Select(v => new VeiculoDTO
                {
                    Id = v.Id,
                    Concessionaria = v.IdConcessionaria,
                    Ano = v.Ano,
                    Cor = v.Cor,
                    Quilometragem = v.Quilometragem,
                    Preco = v.Preco,
                    Placa = v.Placa
                }).ToList();
        }

        public void Delete(uint v)
        {
            throw new ServiceException("Método não implementado.");
        }
    }
}