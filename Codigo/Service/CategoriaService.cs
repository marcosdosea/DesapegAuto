using Core;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class CategoriaService : ICategoriaService
    {
        private readonly DesapegAutoContext context;

        public CategoriaService(DesapegAutoContext context)
        {
            this.context = context;
        }

        public int Create(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
            {
                throw new ServiceException("Erro: Nome da categoria e obrigatorio.");
            }

            categoria.Nome = categoria.Nome.Trim();

            Categoria? categoriaExistente = context.Categoria
                .FirstOrDefault(c => c.Nome.ToLower() == categoria.Nome.ToLower());

            if (categoriaExistente != null)
            {
                throw new ServiceException("Erro: Categoria ja existente na base de dados.");
            }

            context.Categoria.Add(categoria);
            context.SaveChanges();
            return categoria.Id;
        }

        public void Edit(Categoria categoria)
        {
            var categoriaExistente = context.Categoria.Find(categoria.Id);
            if (categoriaExistente == null)
            {
                throw new ServiceException("Erro: Categoria nao encontrada. A operacao foi cancelada.");
            }

            if (string.IsNullOrWhiteSpace(categoria.Nome))
            {
                throw new ServiceException("Erro: Nome da categoria e obrigatorio.");
            }

            var nomeCategoria = categoria.Nome.Trim();
            var categoriaMesmoNome = context.Categoria
                .FirstOrDefault(c => c.Id != categoria.Id && c.Nome.ToLower() == nomeCategoria.ToLower());
            if (categoriaMesmoNome != null)
            {
                throw new ServiceException("Erro: Ja existe outra categoria com este nome.");
            }

            categoriaExistente.Nome = nomeCategoria;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var categoria = context.Categoria.Find(id);
            if (categoria == null)
            {
                throw new ServiceException("Erro: Categoria nao encontrada. A operacao foi cancelada.");
            }

            context.Remove(categoria);
            context.SaveChanges();
        }

        public Categoria? Get(int id)
        {
            return context.Categoria.Find(id);
        }

        public IEnumerable<Categoria> GetAll()
        {
            return context.Categoria.AsNoTracking();
        }

        public IEnumerable<Categoria> GetByNome(string nome)
        {
            return context.Categoria
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}
