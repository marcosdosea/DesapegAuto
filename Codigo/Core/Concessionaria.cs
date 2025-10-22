using System;
using System.Collections.Generic;

namespace Core;

public partial class Concessionaria
{
    public int Id { get; set; }

    public string Endereco { get; set; } = string.Empty;

    public string Cnpj { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public int IdEndereco { get; set; }
}
