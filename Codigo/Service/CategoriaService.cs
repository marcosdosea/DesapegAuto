using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Core.Exceptions;
using System;
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

        /// <summary>
        /// Cadastrar uma nova categoria na base de dados
        /// </summary>
        /// <param name="categoria">Dados da categoria</param>
        /// <returns>ID da nova categoria cadastrada</returns>
        public int Create(Categoria categoria)
        {
            Categoria? categoriaExistente = context.Categoria
                .FirstOrDefault(c => c.Nome.ToLower() == categoria.Nome.ToLower());

            if (categoriaExistente != null)
            {
                throw new ServiceException("Erro: Categoria já existente na base de dados.");
            }

            context.Add(categoria);
            context.SaveChanges();
            return categoria.Id;
        }

        /// <summary>
        /// Editar os dados de uma categoria existente
        /// </summary>
        /// <param name="categoria">Dados da categoria a serem atualizados</param>
        public void Edit(Categoria categoria)
        {
            var categoriaExistente = context.Categoria.Find(categoria.Id);
            if (categoriaExistente == null)
            {
                throw new ServiceException("Erro: Categoria não encontrada. A operação foi cancelada.");
            }

            var categoriaMesmoNome = context.Categoria
                .FirstOrDefault(c => c.Id != categoria.Id && c.Nome.ToLower() == categoria.Nome.ToLower());
            if (categoriaMesmoNome != null)
            {
                throw new ServiceException("Erro: Já existe outra categoria com este nome.");
            }

            context.Update(categoria);
            context.SaveChanges();
        }

        /// <summary>
        /// Apagar uma categoria da base de dados
        /// </summary>
        /// <param name="id">ID da categoria a ser apagada</param>
        public void Delete(int id)
        {
            var categoria = context.Categoria.Find(id);
            if (categoria == null)
            {
                throw new ServiceException("Erro: Categoria não encontrada. A operação foi cancelada.");
            }

            context.Remove(categoria);
            context.SaveChanges();
        }

        /// <summary>
        /// Obter os dados de uma categoria específica pelo ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Dados da categoria encontrada ou null</returns>
        public Categoria? Get(int id)
        {
            return context.Categoria.Find(id);
        }

        /// <summary>
        /// Obter todas as categorias cadastradas
        /// </summary>
        /// <returns>Lista de todas as categorias</returns>
        public IEnumerable<Categoria> GetAll()
        {
            return context.Categoria.AsNoTracking();
        }

        /// <summary>
        /// Buscar categorias que contenham o nome pesquisado (case insensitive)
        /// </summary>
        /// <param name="nome">Nome a ser pesquisado</param>
        /// <returns>Lista de categorias encontradas</returns>
        public IEnumerable<Categoria> GetByNome(string nome)
        {
            return context.Categoria
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}
