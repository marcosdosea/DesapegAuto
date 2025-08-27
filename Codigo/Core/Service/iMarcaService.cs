using Core;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IMarcaService
    {
        int Create(Marca marca);
        void Edit(Marca marca);
        void Delete(int id);
        Marca? Get(int id);
        IEnumerable<Marca> GetAll();
        IEnumerable<Marca> GetByNome(string nome);
    }
}
