using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IModeloService
    {
        int Create(Modelo modelo);
        void Edit(Modelo modelo);
        void Delete(int id);
        Modelo? Get(int id);
        IEnumerable<Modelo> GetAll();
        IEnumerable<ModeloDTO> GetByMarca(int idMarca);
        IEnumerable<ModeloDTO> GetByCategoria(string categoria);
        IEnumerable<ModeloDTO> GetByNome(string nome);
    }
}
