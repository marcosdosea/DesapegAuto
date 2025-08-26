using Core;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IMarcaService
    {
        int Create(Marca marca);
        IEnumerable<Marca> GetAll();
        IEnumerable<Marca> GetByNome(string nome);
    }
}
