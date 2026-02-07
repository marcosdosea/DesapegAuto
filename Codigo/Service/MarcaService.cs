using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class MarcaService : IMarcaService
    {
        private readonly DesapegAutoContext context;

        public MarcaService(DesapegAutoContext context)
        {
            this.context = context;
        }

        public int Create(Marca marca)
        {
            if (string.IsNullOrWhiteSpace(marca.Nome))
            {
                throw new ServiceException("Erro: Nome da marca e obrigatorio.");
            }

            marca.Nome = marca.Nome.Trim();

            Marca? marcaExistente = context.Marcas
                .FirstOrDefault(m => m.Nome.ToLower() == marca.Nome.ToLower());

            if (marcaExistente != null)
            {
                throw new ServiceException("Erro: Marca ja existente na base de dados.");
            }

            context.Marcas.Add(marca);
            context.SaveChanges();
            return marca.Id;
        }

        public void Edit(Marca marca)
        {
            var marcaExistente = context.Marcas.Find(marca.Id);
            if (marcaExistente == null)
            {
                throw new ServiceException("Erro: Marca nao encontrada. A operacao foi cancelada.");
            }

            if (string.IsNullOrWhiteSpace(marca.Nome))
            {
                throw new ServiceException("Erro: Nome da marca e obrigatorio.");
            }

            var nomeMarca = marca.Nome.Trim();
            var marcaMesmoNome = context.Marcas
                .FirstOrDefault(m => m.Id != marca.Id && m.Nome.ToLower() == nomeMarca.ToLower());
            if (marcaMesmoNome != null)
            {
                throw new ServiceException("Erro: Ja existe outra marca com este nome.");
            }

            marcaExistente.Nome = nomeMarca;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var marca = context.Marcas.Find(id);
            if (marca == null)
            {
                throw new ServiceException("Erro: Marca nao encontrada. A operacao foi cancelada.");
            }

            context.Remove(marca);
            context.SaveChanges();
        }

        public Marca? Get(int id)
        {
            return context.Marcas.Find(id);
        }

        public IEnumerable<Marca> GetAll()
        {
            return context.Marcas.AsNoTracking();
        }

        public IEnumerable<Marca> GetByNome(string nome)
        {
            return context.Marcas
                .Where(m => m.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}
