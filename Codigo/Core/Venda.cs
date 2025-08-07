using System;
using System.Collections.Generic;

namespace Core;

public partial class Venda
{
    public int Id { get; set; }

    public int IdConcessionaria { get; set; }

    public int IdUsuario { get; set; }

    public DateTime DataVenda { get; set; }

    public decimal ValorFinal { get; set; }

    public string FormaPagamento { get; set; } = null!;
}
