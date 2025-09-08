using Core;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IConcessionariaService
    {
        int Create(Concessionaria concessionaria);
        void Edit(Concessionaria concessionaria);
        void Delete(int idConcessionaria);
        Concessionaria? Get(int idConcessionaria);
        IEnumerable<Concessionaria> GetAll();
        IEnumerable<Concessionaria> GetByNome(string nome);
    }
}