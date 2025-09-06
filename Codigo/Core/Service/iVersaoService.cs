using Core.DTO;

namespace Core.Service
{
    public interface IVersaoService
    {
        int Create(Versao versao);
        void Edit(Versao versao);
        void Delete(int idVersao);
        Versao? Get(int idVersao);
        IEnumerable<Versao> GetAll();
        IEnumerable<VersaoDTO> GetByNome(string nome);
    }
}