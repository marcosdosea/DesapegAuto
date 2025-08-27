using Core;
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

        /// <summary>
        /// Cadastrar uma nova marca na base de dados
        /// </summary>
        /// <param name="marca">Dados da marca</param>
        /// <returns>ID da nova marca cadastrada</returns>
        public int Create(Marca marca)
        {
            Marca? marcaExistente = context.Marcas
                .FirstOrDefault(m => m.Nome.ToLower() == marca.Nome.ToLower());

            if (marcaExistente != null)
            {
                throw new Exception("Marca já existente na base de dados.");
            }

            context.Add(marca);
            context.SaveChanges();
            return marca.Id;
        }

        /// <summary>
        /// Editar os dados de uma marca existente
        /// </summary>
        /// <param name="marca">Dados da marca a serem atualizados</param>
        public void Edit(Marca marca)
        {
            context.Update(marca);
            context.SaveChanges();
        }

        /// <summary>
        /// Apagar uma marca da base de dados
        /// </summary>
        /// <param name="id">ID da marca a ser apagada</param>
        public void Delete(int id)
        {
            var marca = context.Marcas.Find(id);
            if (marca != null)
            {
                context.Remove(marca);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Obter os dados de uma marca específica pelo ID
        /// </summary>
        /// <param name="id">ID da marca</param>
        /// <returns>Dados da marca encontrada ou null</returns>
        public Marca? Get(int id)
        {
            return context.Marcas.Find(id);
        }

        /// <summary>
        /// Obter todas as marcas cadastradas
        /// </summary>
        /// <returns>Lista de todas as marcas</returns>
        public IEnumerable<Marca> GetAll()
        {
            return context.Marcas.AsNoTracking();
        }

        /// <summary>
        /// Buscar marcas que contenham o nome pesquisado
        /// </summary>
        /// <param name="nome">Nome a ser pesquisado</param>
        /// <returns>Lista de marcas encontradas</returns>
        public IEnumerable<Marca> GetByNome(string nome)
        {
            return context.Marcas
                .Where(m => m.Nome.Contains(nome))
                .AsNoTracking()
                .ToList();
        }
    }
}
