using Core;
using Core.DTO;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class VendaService : IVendaService
    {
        private readonly DesapegAutoContext context;

        public VendaService(DesapegAutoContext context)
        {
            this.context = context;
        }

        public int Create(Venda venda)
        {
            context.Add(venda);
            context.SaveChanges();
            return venda.Id;
        }

        public void Edit(Venda venda)
        {
            var vendaExistente = context.Venda.Find(venda.Id);
            if (vendaExistente == null)
            {
                throw new ServiceException("Erro: Venda não encontrada. A operação foi cancelada.");
            }

            vendaExistente.IdConcessionaria = venda.IdConcessionaria;
            vendaExistente.IdPessoa = venda.IdPessoa;
            vendaExistente.DataVenda = venda.DataVenda;
            vendaExistente.ValorFinal = venda.ValorFinal;
            vendaExistente.FormaPagamento = venda.FormaPagamento;
            context.SaveChanges();
        }

        public void Delete(int idVenda)
        {
            var venda = context.Venda.Find(idVenda);
            if (venda == null)
            {
                throw new ServiceException("Erro: Venda não encontrada. A operação foi cancelada.");
            }

            context.Remove(venda);
            context.SaveChanges();
        }

        public Venda? Get(int idVenda)
        {
            return context.Venda.Find(idVenda);
        }

        public IEnumerable<Venda> GetAll()
        {
            return context.Venda.AsNoTracking();
        }

        public IEnumerable<VendaDTO> GetAllDTO()
        {
            // Consulta agora inclui a junção com Pessoas
            var query = from venda in context.Venda
                        join concessionaria in context.Concessionaria on venda.IdConcessionaria equals concessionaria.Id
                        join pessoa in context.Pessoas on venda.IdPessoa equals pessoa.Id
                        join anuncio in context.Anuncios on venda.Id equals anuncio.IdVenda into anuncios
                        from anuncio in anuncios.DefaultIfEmpty()
                        select new VendaDTO
                        {
                            Id = venda.Id,
                            DataVenda = venda.DataVenda,
                            ValorFinal = venda.ValorFinal,
                            FormaPagamento = venda.FormaPagamento,
                            NomeConcessionaria = concessionaria.Nome,
                            NomePessoa = pessoa.Nome,
                            StatusAnuncio = anuncio != null ? anuncio.StatusAnuncio : null
                        };

            return query.AsNoTracking().ToList();
        }
    }
}