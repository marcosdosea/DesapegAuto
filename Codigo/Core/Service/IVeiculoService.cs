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
    }
}
