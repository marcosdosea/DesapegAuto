using System;
using System.Collections.Generic;

namespace Core;

public partial class Anuncio
{
    public int Id { get; set; }

    public int IdVeiculo { get; set; }

    public int IdVenda { get; set; }

    public DateTime DataPublicacao { get; set; }

    public string StatusAnuncio { get; set; } = null!;

    public int? Interacoes { get; set; }

    public int Visualizacoes { get; set; }

    public string Descricao { get; set; } = null!;

    public string Opcionais { get; set; } = null!;
}
