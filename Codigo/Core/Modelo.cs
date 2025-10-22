using System;
using System.Collections.Generic;

namespace Core;

public partial class Modelo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    // Provide default values so tests can create Modelo with only Id and Nome
    public string Categoria { get; set; } = string.Empty;

    public string Versoes { get; set; } = string.Empty;

    public int IdMarca { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    // Initialize collection to avoid null reference
    public virtual ICollection<Versao> Versaos { get; set; } = new List<Versao>();
}
