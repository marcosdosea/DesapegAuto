namespace Core.DTO
{
    /// <summary>
    /// DTO para listagem e visualização de pessoas.
    /// </summary>
    public class PessoaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
}