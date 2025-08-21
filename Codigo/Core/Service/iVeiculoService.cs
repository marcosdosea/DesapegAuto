using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IVeiculoService
    {
        int Create(Veiculo veiculo);
        void Edit(Veiculo veiculo);
        void Delete(int id);
        Veiculo? Get(int id);
        IEnumerable<Veiculo> GetAll();
        IEnumerable<VeiculoDTO> GetByConcessionaria(int idConcessionaria);
        IEnumerable<VeiculoDTO> GetByAno(int ano);
        IEnumerable<VeiculoDTO> GetByQuilometragem(int quilometragem);
        IEnumerable<VeiculoDTO> GetByPreco(decimal preco);
        IEnumerable<VeiculoDTO> GetByPlaca(string placa);
    }
}
