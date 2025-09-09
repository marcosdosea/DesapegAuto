using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    /// <summary>
    /// Implementa os serviços de negócio para a entidade Pessoa.
    /// </summary>
    public class PessoaService : IPessoaService
    {
        private readonly DesapegAutoContext context;

        public PessoaService(DesapegAutoContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cadastra uma nova pessoa no sistema.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa com os dados para cadastro.</param>
        /// <returns>O ID da pessoa cadastrada.</returns>
        public int Create(Pessoa pessoa)
        {
            context.Add(pessoa);
            context.SaveChanges();
            return pessoa.Id;
        }

        /// <summary>
        /// Edita os dados de uma pessoa existente.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa com os dados atualizados.</param>
        public void Edit(Pessoa pessoa)
        {
            context.Update(pessoa);
            context.SaveChanges();
        }

        /// <summary>
        /// Remove uma pessoa do sistema.
        /// </summary>
        /// <param name="idPessoa">O ID da pessoa a ser removida.</param>
        public void Delete(int idPessoa)
        {
            var pessoa = context.Pessoas.Find(idPessoa);
            if (pessoa != null)
            {
                context.Remove(pessoa);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Busca uma pessoa pelo seu ID.
        /// </summary>
        /// <param name="idPessoa">O ID da pessoa.</param>
        /// <returns>A pessoa encontrada ou null se não existir.</returns>
        public Pessoa? Get(int idPessoa)
        {
            return context.Pessoas.Find(idPessoa);
        }

        /// <summary>
        /// Retorna uma lista com todas as pessoas cadastradas.
        /// </summary>
        /// <returns>Uma coleção de todas as pessoas.</returns>
        public IEnumerable<Pessoa> GetAll()
        {
            return context.Pessoas.AsNoTracking();
        }

        /// <summary>
        /// Busca pessoas pelo nome.
        /// </summary>
        /// <param name="nome">O nome a ser buscado.</param>
        /// <returns>Uma coleção de pessoas que correspondem ao critério de busca.</returns>
        public IEnumerable<Pessoa> GetByNome(string nome)
        {
            return context.Pessoas
                .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}