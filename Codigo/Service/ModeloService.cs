using Core;
using Core.DTO;
using Core.Exceptions;
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
            if (string.IsNullOrWhiteSpace(modelo.Nome))
            {
                throw new ServiceException("Erro: O nome do modelo e obrigatorio.");
            }

            modelo.Nome = modelo.Nome.Trim();
            modelo.Versoes = (modelo.Versoes ?? string.Empty).Trim();

            if (!context.Marcas.Any(m => m.Id == modelo.IdMarca))
            {
                throw new ServiceException("Erro: Marca invalida para o modelo.");
            }

            var categoria = context.Categoria.FirstOrDefault(c => c.Id == modelo.IdCategoria);
            if (categoria == null)
            {
                throw new ServiceException("Erro: Categoria invalida para o modelo.");
            }

            modelo.Categoria = categoria.Nome;

            var nomeModelo = modelo.Nome.ToLower();
            var modeloExistente = context.Modelos
                .FirstOrDefault(m => m.Nome.ToLower() == nomeModelo && m.IdMarca == modelo.IdMarca);

            if (modeloExistente != null)
            {
                throw new ServiceException("Erro: Modelo ja existente para esta marca.");
            }

            context.Modelos.Add(modelo);
            context.SaveChanges();
            return modelo.Id;
        }

        public void Edit(Modelo modelo)
        {
            var modeloExistente = context.Modelos.Find(modelo.Id);
            if (modeloExistente == null)
            {
                throw new ServiceException("Erro: Modelo nao encontrado. A operacao foi cancelada.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Nome))
            {
                throw new ServiceException("Erro: O nome do modelo e obrigatorio.");
            }

            modelo.Nome = modelo.Nome.Trim();
            modelo.Versoes = (modelo.Versoes ?? string.Empty).Trim();

            if (!context.Marcas.Any(m => m.Id == modelo.IdMarca))
            {
                throw new ServiceException("Erro: Marca invalida para o modelo.");
            }

            var categoria = context.Categoria.FirstOrDefault(c => c.Id == modelo.IdCategoria);
            if (categoria == null)
            {
                throw new ServiceException("Erro: Categoria invalida para o modelo.");
            }

            var modeloMesmoNome = context.Modelos
                .FirstOrDefault(m => m.Id != modelo.Id &&
                                     m.Nome.ToLower() == modelo.Nome.ToLower() &&
                                     m.IdMarca == modelo.IdMarca);
            if (modeloMesmoNome != null)
            {
                throw new ServiceException("Erro: Ja existe outro modelo com este nome para esta marca.");
            }

            modeloExistente.Nome = modelo.Nome;
            modeloExistente.IdMarca = modelo.IdMarca;
            modeloExistente.IdCategoria = modelo.IdCategoria;
            modeloExistente.Categoria = categoria.Nome;
            modeloExistente.Versoes = modelo.Versoes;

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var modelo = context.Modelos.Find(id);
            if (modelo == null)
            {
                throw new ServiceException("Erro: Modelo nao encontrado. A operacao foi cancelada.");
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
                    IdCategoria = m.IdCategoria
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
                    IdCategoria = m.IdCategoria
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
                    IdCategoria = m.IdCategoria
                }).ToList();
        }
    }
}
