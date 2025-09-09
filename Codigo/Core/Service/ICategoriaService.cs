using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface ICategoriaService
    {
        int Create(Categoria categoria);
        void Edit(Categoria categoria);
        void Delete(int id);
        Categoria? Get(int id);
        IEnumerable<Categoria> GetAll();
        IEnumerable<Categoria> GetByNome(string nome);
    }
}
