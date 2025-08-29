using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IModeloService
    {
        int Create(Modelo modelo);
        void Edit(Modelo modelo);
        void Delete(int id);
        Modelo? Get(int id);
        IEnumerable<Modelo> GetAll();
    }
}
