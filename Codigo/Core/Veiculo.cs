using System;
using System.Collections.Generic;

namespace Core;

public partial class Veiculo
{
    public int Id { get; set; }

    public int IdConcessionaria { get; set; }

    public int IdModelo { get; set; }

    public int IdMarca { get; set; }

    public int Ano { get; set; }

    public string Cor { get; set; } = null!;

    public int Quilometragem { get; set; }

    public decimal Preco { get; set; }

    public string Placa { get; set; } = null!;
}
