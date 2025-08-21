using Core;
using Core.DTO;
using Core.Service;
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
            _context.Update(veiculo);
            _context.SaveChanges();
        }

        /// <summary>
        /// Remover um veículo da base de dados
        /// </summary>
        /// <param name="id">ID do veículo</param>
        public void Delete(int id)
        {
            var veiculo = _context.Veiculos.Find(id);
            if (veiculo != null)
            {
                _context.Remove(veiculo);
                _context.SaveChanges();
            }
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

        // --- Métodos de busca específicos que retornam DTO ---

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

        public IEnumerable<VeiculoDTO> GetByQuilometragem(int quilometragem)
        {
            // Busca veículos com quilometragem menor ou igual à informada
            return _context.Veiculos
                .Where(v => v.Quilometragem <= quilometragem)
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

        public IEnumerable<VeiculoDTO> GetByPreco(decimal preco)
        {
            // Busca veículos com preço menor ou igual ao informado
            return _context.Veiculos
                .Where(v => v.Preco <= preco)
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

        void IVeiculoService.Delete(uint v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VeiculoDTO> GetByQuilometragem(int quilometragem, int v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VeiculoDTO> GetByPreco(decimal preco, int v)
        {
            throw new NotImplementedException();
        }
    }
}