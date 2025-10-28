using Core;
using Core.DTO;
using Core.Service;
using Core.Exceptions;
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
            var modeloExistente = context.Modelos
                .FirstOrDefault(m => m.Nome.ToLower() == modelo.Nome.ToLower() && m.IdMarca == modelo.IdMarca);
                
            if (modeloExistente != null)
            {
                throw new ServiceException("Erro: Modelo já existente para esta marca.");
            }

            context.Add(modelo);
            context.SaveChanges();
            return modelo.Id;
        }

        public void Edit(Modelo modelo)
        {
            var modeloExistente = context.Modelos.Find(modelo.Id);
            if (modeloExistente == null)
            {
                throw new ServiceException("Erro: Modelo não encontrado. A operação foi cancelada.");
            }

            var modeloMesmoNome = context.Modelos
                .FirstOrDefault(m => m.Id != modelo.Id && 
                                   m.Nome.ToLower() == modelo.Nome.ToLower() && 
                                   m.IdMarca == modelo.IdMarca);
            if (modeloMesmoNome != null)
            {
                throw new ServiceException("Erro: Já existe outro modelo com este nome para esta marca.");
            }

            context.Update(modelo);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var modelo = context.Modelos.Find(id);
            if (modelo == null)
            {
                throw new ServiceException("Erro: Modelo não encontrado. A operação foi cancelada.");
            }

            context.Remove(modelo);
            context.SaveChanges();
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
