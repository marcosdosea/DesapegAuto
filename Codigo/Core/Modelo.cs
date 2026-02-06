using System;
using System.Collections.Generic;

namespace Core;

public partial class Modelo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    // Keep denormalized category name for legacy screens.
    public string Categoria { get; set; } = string.Empty;

    public string Versoes { get; set; } = string.Empty;

    public int IdCategoria { get; set; }

    public int IdMarca { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    // Initialize collection to avoid null reference
    public virtual ICollection<Versao> Versaos { get; set; } = new List<Versao>();
}
