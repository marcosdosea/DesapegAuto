using System.Collections.Generic;

namespace Core.Service
{
    public interface IVeiculoService
    {
        uint Create(Veiculo veiculo);
        void Edit(Veiculo veiculo);
        void Delete(uint id);
        Veiculo? Get(uint id);
        IEnumerable<Veiculo> GetAll();
    }
}
