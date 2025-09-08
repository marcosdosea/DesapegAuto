using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IVendaService
    {
        int Create(Venda venda);
        void Edit(Venda venda);
        void Delete(int idVenda);
        Venda? Get(int idVenda);
        IEnumerable<Venda> GetAll();
        IEnumerable<VendaDTO> GetAllDTO();
    }
}