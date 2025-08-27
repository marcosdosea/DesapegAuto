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
            // Regra de negócio: Verifica se já existe uma marca com o mesmo nome (ignorando maiúsculas/minúsculas)
            var marcaExistente = context.Marcas
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
