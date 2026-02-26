using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            concessionaria.Cnpj = NormalizeCnpj(concessionaria.Cnpj);

            context.Add(concessionaria);
            context.SaveChanges();
            return concessionaria.Id;
        }

        public void Edit(Concessionaria concessionaria)
        {
            concessionaria.Cnpj = NormalizeCnpj(concessionaria.Cnpj);

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

        private static string NormalizeCnpj(string? rawCnpj)
        {
            if (string.IsNullOrWhiteSpace(rawCnpj))
            {
                throw new ServiceException("Erro: CNPJ obrigatorio.");
            }

            var digits = Regex.Replace(rawCnpj, "[^0-9]", string.Empty);
            if (digits.Length != 14)
            {
                throw new ServiceException("Erro: CNPJ invalido. Use 14 digitos.");
            }

            return digits;
        }
    }
}
