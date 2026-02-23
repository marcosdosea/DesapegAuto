namespace Core.DTO
{
    /// <summary>
    /// DTO para listagem e visualização de pessoas.
    /// </summary>
    public class PessoaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
    }
}