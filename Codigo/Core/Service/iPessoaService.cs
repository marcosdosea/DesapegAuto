using Core;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IPessoaService
    {
        int Create(Pessoa pessoa);
        void Edit(Pessoa pessoa);
        void Delete(int idPessoa);
        Pessoa? Get(int idPessoa);
        Pessoa? GetByEmail(string email);
        Pessoa? GetByCpf(string cpf);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<Pessoa> GetByNome(string nome);
    }
}