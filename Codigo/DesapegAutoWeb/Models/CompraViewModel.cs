using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class CompraViewModel
    {
        [Required]
        public int IdAnuncio { get; set; }

        [Required(ErrorMessage = "O nome e obrigatorio.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail e obrigatorio.")]
        [EmailAddress(ErrorMessage = "Email invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF e obrigatorio.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "CPF invalido.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone e obrigatorio.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A forma de pagamento e obrigatoria.")]
        [StringLength(45, ErrorMessage = "A forma de pagamento deve ter no maximo 45 caracteres.")]
        public string FormaPagamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor final e obrigatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor final deve ser maior que zero.")]
        public decimal ValorFinal { get; set; }
    }
}
