using Core;
using Core.Service;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    /// <summary>
    /// Implementa os servi�os de neg�cio para a entidade Pessoa.
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
            var pessoaExistente = context.Pessoas
                .FirstOrDefault(p => p.Cpf.ToLower() == pessoa.Cpf.ToLower());

            if (pessoaExistente != null)
            {
                throw new ServiceException("Erro: CPF j� cadastrado no sistema.");
            }

            var emailExistente = context.Pessoas
                .FirstOrDefault(p => p.Email.ToLower() == pessoa.Email.ToLower());

            if (emailExistente != null)
            {
                throw new ServiceException("Erro: E-mail j� cadastrado no sistema.");
            }

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
            var pessoaExistente = context.Pessoas.Find(pessoa.Id);
            if (pessoaExistente == null)
            {
                throw new ServiceException("Erro: Pessoa n�o encontrada. A opera��o foi cancelada.");
            }

            var cpfExistente = context.Pessoas
                .FirstOrDefault(p => p.Id != pessoa.Id && 
                                   p.Cpf.ToLower() == pessoa.Cpf.ToLower());
            if (cpfExistente != null)
            {
                throw new ServiceException("Erro: CPF j� cadastrado para outra pessoa.");
            }

            var emailExistente = context.Pessoas
                .FirstOrDefault(p => p.Id != pessoa.Id && 
                                   p.Email.ToLower() == pessoa.Email.ToLower());
            if (emailExistente != null)
            {
                throw new ServiceException("Erro: E-mail j� cadastrado para outra pessoa.");
            }

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
            if (pessoa == null)
            {
                throw new ServiceException("Erro: Pessoa n�o encontrada. A opera��o foi cancelada.");
            }

            context.Remove(pessoa);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca uma pessoa pelo seu ID.
        /// </summary>
        /// <param name="idPessoa">O ID da pessoa.</param>
        /// <returns>A pessoa encontrada ou null se n�o existir.</returns>
        public Pessoa? Get(int idPessoa)
        {
            return context.Pessoas.Find(idPessoa);
        }

        public Pessoa? GetByEmail(string email)
        {
            return context.Pessoas
                .AsNoTracking()
                .FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
        }

        public Pessoa? GetByCpf(string cpf)
        {
            return context.Pessoas
                .AsNoTracking()
                .FirstOrDefault(p => p.Cpf.ToLower() == cpf.ToLower());
        }

        /// <summary>
        /// Retorna uma lista com todas as pessoas cadastradas.
        /// </summary>
        /// <returns>Uma cole��o de todas as pessoas.</returns>
        public IEnumerable<Pessoa> GetAll()
        {
            return context.Pessoas.AsNoTracking();
        }

        /// <summary>
        /// Busca pessoas pelo nome.
        /// </summary>
        /// <param name="nome">O nome a ser buscado.</param>
        /// <returns>Uma cole��o de pessoas que correspondem ao crit�rio de busca.</returns>
        public IEnumerable<Pessoa> GetByNome(string nome)
        {
            return context.Pessoas
                .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToList();
        }
    }
}