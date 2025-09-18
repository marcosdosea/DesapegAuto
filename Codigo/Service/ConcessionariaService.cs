using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ConcessionariaService : IConcessionariaService
    {
        private readonly DesapegAutoContext context;

        public ConcessionariaService(DesapegAutoContext context)
        {
            this.context = context;
        }

        public int Create(Concessionaria concessionaria)
        {
            context.Add(concessionaria);
            context.SaveChanges();
            return concessionaria.Id;
        }

        public void Edit(Concessionaria concessionaria)
        {
            context.Update(concessionaria);
            context.SaveChanges();
        }

        public void Delete(int idConcessionaria)
        {
            var c = context.Concessionaria.Find(idConcessionaria);
            if (c != null)
            {
                context.Remove(c);
                context.SaveChanges();
            }
        }

        public Concessionaria? Get(int idConcessionaria)
        {
            return context.Concessionaria.Find(idConcessionaria);
        }

        public IEnumerable<Concessionaria> GetAll()
        {
            return context.Concessionaria.AsNoTracking();
        }

        public IEnumerable<Concessionaria> GetByNome(string nome)
        {
            return context.Concessionaria
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}
