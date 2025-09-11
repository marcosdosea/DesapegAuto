using System.Collections.Generic;

namespace Core.Service
{
    public interface IAnuncioService
    {
        int Create(Anuncio anuncio);
        void Edit(Anuncio anuncio);
        void Delete(int id);
        Anuncio? Get(int id);
        IEnumerable<Anuncio> GetAll();
    }
}
