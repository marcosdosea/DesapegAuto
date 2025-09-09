using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ModeloService : IModeloService
    {
        private readonly DesapegAutoContext context;

        public ModeloService(DesapegAutoContext context)
        {
            this.context = context;
        }

        public int Create(Modelo modelo)
        {
            context.Add(modelo);
            context.SaveChanges();
            return modelo.Id;
        }

        public void Edit(Modelo modelo)
        {
            context.Update(modelo);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var modelo = context.Modelos.Find(id);
            if (modelo != null)
            {
                context.Remove(modelo);
                context.SaveChanges();
            }
        }

        public Modelo? Get(int id)
        {
            return context.Modelos.Find(id);
        }

        public IEnumerable<Modelo> GetAll()
        {
            return context.Modelos.AsNoTracking();
        }

        public IEnumerable<ModeloDTO> GetByMarca(int idMarca)
        {
            return context.Modelos
                .Where(m => m.IdMarca == idMarca)
                .Select(m => new ModeloDTO
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Versoes = m.Versoes,
                    IdMarca = m.IdMarca,
                    IdCategoria = 0 // Ajuste conforme necessário
                }).ToList();
        }

        public IEnumerable<ModeloDTO> GetByCategoria(string categoria)
        {
            return context.Modelos
                .Where(m => m.Categoria.ToLower().Contains(categoria.ToLower()))
                .Select(m => new ModeloDTO
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Versoes = m.Versoes,
                    IdMarca = m.IdMarca,
                    IdCategoria = 0 // Ajuste conforme necessário
                }).ToList();
        }

        public IEnumerable<ModeloDTO> GetByNome(string nome)
        {
            return context.Modelos
                .Where(m => m.Nome.ToLower().Contains(nome.ToLower()))
                .Select(m => new ModeloDTO
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Versoes = m.Versoes,
                    IdMarca = m.IdMarca,
                    IdCategoria = 0 // Ajuste conforme necessário
                }).ToList();
        }
    }
}
