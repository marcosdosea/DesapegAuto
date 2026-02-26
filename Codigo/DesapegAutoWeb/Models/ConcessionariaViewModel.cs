using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ConcessionariaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome e obrigatorio.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O CNPJ e obrigatorio.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 digitos.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ invalido.")]
        public string Cnpj { get; set; } = null!;

        [Required(ErrorMessage = "O email e obrigatorio.")]
        [EmailAddress(ErrorMessage = "Email invalido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O telefone e obrigatorio.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone invalido.")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "A senha e obrigatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 20 caracteres.")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d).+$", ErrorMessage = "A senha deve conter ao menos uma letra e um numero.")]
        public string Senha { get; set; } = null!;

        [Required(ErrorMessage = "A confirmacao de senha e obrigatoria.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "A confirmacao de senha nao confere.")]
        public string ConfirmarSenha { get; set; } = null!;

        [Required(ErrorMessage = "O endereco e obrigatorio.")]
        [StringLength(80, ErrorMessage = "O endereco deve ter no maximo 80 caracteres.")]
        public string Endereco { get; set; } = null!;
    }
}
