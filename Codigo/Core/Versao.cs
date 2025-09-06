using System;
using System.Collections.Generic;

namespace Core;

public partial class Versao
{
    public int Id { get; set; }

    public int IdModelo { get; set; }

    public string Nome { get; set; } = null!;

    public virtual Modelo IdModeloNavigation { get; set; } = null!;
}
