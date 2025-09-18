using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ConcessionariaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        public string Cnpj { get; set; } = null!;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = null!;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(80, ErrorMessage = "O endereço deve ter no máximo 80 caracteres.")]
        public string Endereco { get; set; } = null!;
    }
}
