using System;
using System.Collections.Generic;

namespace Core;

public partial class Modelo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string Versoes { get; set; } = null!;

    public int IdMarca { get; set; }
}
