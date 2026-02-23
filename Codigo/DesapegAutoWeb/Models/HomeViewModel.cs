namespace DesapegAutoWeb.Models
{
    /// <summary>
    /// Represents a brand group shown on the home page "Por Marca" section.
    /// </summary>
    public class MarcaGroupViewModel
    {
        public int IdMarca { get; set; }
        public string NomeMarca { get; set; } = string.Empty;
        public int TotalAnuncios { get; set; }
        public List<AnuncioViewModel> Anuncios { get; set; } = new();
    }

    /// <summary>
    /// Represents a category group shown on the home page "Por Categoria" section.
    /// </summary>
    public class CategoriaGroupViewModel
    {
        public string Categoria { get; set; } = string.Empty;
        public int TotalAnuncios { get; set; }
        public List<AnuncioViewModel> Anuncios { get; set; } = new();
    }

    /// <summary>
    /// Represents a price range group shown on the home page "Por Faixa de Preço" section.
    /// </summary>
    public class FaixaPrecoViewModel
    {
        public string Label { get; set; } = string.Empty;
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }
        public int TotalAnuncios { get; set; }
        public List<AnuncioViewModel> Anuncios { get; set; } = new();
    }

    /// <summary>
    /// Root view-model for the Home/Index page.
    /// Contains the highlights (destaques) and the four special sections.
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>Featured ads shown at the top of the page.</summary>
        public List<AnuncioViewModel> Destaques { get; set; } = new();

        /// <summary>All available ads grouped by brand.</summary>
        public List<MarcaGroupViewModel> PorMarca { get; set; } = new();

        /// <summary>All available ads grouped by category.</summary>
        public List<CategoriaGroupViewModel> PorCategoria { get; set; } = new();

        /// <summary>Price-range buckets with counts.</summary>
        public List<FaixaPrecoViewModel> PorFaixaPreco { get; set; } = new();

        /// <summary>The N most-recently published ads.</summary>
        public List<AnuncioViewModel> MaisRecentes { get; set; } = new();
    }
}
