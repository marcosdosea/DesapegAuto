namespace Core.DTO
{
    public class VersaoDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? NomeModelo { get; set; } // Adicionado para facilitar a exibição do nome do modelo
    }
}