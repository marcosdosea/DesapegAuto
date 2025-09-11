using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class AnuncioService : IAnuncioService
    {
        private readonly DesapegAutoContext _context;

        public AnuncioService(DesapegAutoContext context)
        {
            _context = context;
        }

        public int Create(Anuncio anuncio)
        {
            // Inicializa valores padrão
            anuncio.DataPublicacao = System.DateTime.Now;
            anuncio.StatusAnuncio = anuncio.StatusAnuncio ?? "D"; // D = Disponível
            anuncio.Visualizacoes = anuncio.Visualizacoes;

            _context.Add(anuncio);
            _context.SaveChanges();
            return anuncio.Id;
        }

        public void Edit(Anuncio anuncio)
        {
            _context.Update(anuncio);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var anuncio = _context.Anuncios.Find(id);
            if (anuncio != null)
            {
                _context.Remove(anuncio);
                _context.SaveChanges();
            }
        }

        public Anuncio? Get(int id)
        {
            return _context.Anuncios.Find(id);
        }

        public IEnumerable<Anuncio> GetAll()
        {
            return _context.Anuncios.AsNoTracking();
        }
    }
}
