using Core;
using Core.DTO;
using Core.Exceptions;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class VersaoService : IVersaoService
    {
        private readonly DesapegAutoContext context;

        public VersaoService(DesapegAutoContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cadastrar uma nova versão de veículo na base de dados
        /// </summary>
        /// <param name="versao">Dados da versão</param>
        /// <returns>ID da versão</returns>
        public int Create(Versao versao)
        {
            context.Add(versao);
            context.SaveChanges();
            return versao.Id;
        }

        /// <summary>
        /// Editar dados de uma versão na base de dados
        /// </summary>
        /// <param name="versao">Dados da versão</param>
        public void Edit(Versao versao)
        {
            var versaoExistente = context.Versaos.Find(versao.Id);
            if (versaoExistente == null)
            {
                throw new ServiceException("Erro: Versão não encontrada. A operação foi cancelada.");
            }

            versaoExistente.Nome = versao.Nome;
            versaoExistente.IdModelo = versao.IdModelo;
            context.SaveChanges();
        }

        /// <summary>
        /// Remover uma versão da base de dados
        /// </summary>
        /// <param name="idVersao">ID da versão</param>
        public void Delete(int idVersao)
        {
            var versao = context.Versaos.Find(idVersao);

            if (versao == null)
            {
                throw new ServiceException("Erro: Versão não encontrada. A operação foi cancelada.");
            }

            context.Remove(versao);
            context.SaveChanges();
        }

        /// <summary>
        /// Obter dados de uma versão
        /// </summary>
        /// <param name="idVersao">ID da versão</param>
        /// <returns>Dados da versão</returns>
        public Versao? Get(int idVersao)
        {
            return context.Versaos.Find(idVersao);
        }

        /// <summary>
        /// Obter todas as versões cadastradas
        /// </summary>
        /// <returns>Lista de todas as versões</returns>
        public IEnumerable<Versao> GetAll()
        {
            return context.Versaos.AsNoTracking();
        }

        /// <summary>
        /// Obter versões pelo nome
        /// </summary>
        /// <param name="nome">Nome da versão</param>
        /// <returns>Lista de versões com o nome do modelo</returns>
        public IEnumerable<VersaoDTO> GetByNome(string nome)
        {
            return context.Versaos
                .Where(v => v.Nome.Contains(nome))
                .Select(v => new VersaoDTO
                {
                    Id = v.Id,
                    Nome = v.Nome,
                    NomeModelo = v.IdModeloNavigation != null ? v.IdModeloNavigation.Nome : string.Empty
                }).ToList();
        }
    }
}