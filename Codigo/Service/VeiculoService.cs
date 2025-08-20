using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    // Serviço responsável por centralizar as operações de Veículo
    public class VeiculoService : IVeiculoService
    {
        private readonly DesapegAutoContext context;

        // Injeção de dependência do contexto
        public VeiculoService(DesapegAutoContext context)
        {
            this.context = context;
        }

        // Cadastrar veículo e retornar seu Id
        public int Create(Veiculo veiculo)
        {
            context.Add(veiculo);
            context.SaveChanges();
            return veiculo.Id;
        }

        // Excluir veículo se não houver venda pendente associada
        public void Delete(int idVeiculo)
        {
            var veiculo = context.Veiculos.Find(idVeiculo);
            if (veiculo != null)
            {
                var temVendaPendente = context.Venda
                                  .Any(v => v.IdVeiculo == idVeiculo && v.Status == "Pendente");

                if (temVendaPendente)
                    throw new System.Exception("Não é possível remover. Veículo está em uma venda pendente.");

                context.Veiculos.Remove(veiculo);
                context.SaveChanges();
            }
        }

        // Atualizar dados de um veículo
        public void Edit(Veiculo veiculo)
        {
            context.Update(veiculo);
            context.SaveChanges();
        }

        // Buscar veículo por Id
        public Veiculo? Get(int idVeiculo)
        {
            return context.Veiculos.Find(idVeiculo);
        }

        // Listar todos os veículos sem rastrear alterações
        public IEnumerable<Veiculo> GetAll()
        {
            return context.Veiculos.AsNoTracking();
        }
    }
}
