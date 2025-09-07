using System;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class VendaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data da venda é obrigatória.")]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.Date)]
        public DateTime DataVenda { get; set; }

        [Required(ErrorMessage = "O valor final é obrigatório.")]
        [Display(Name = "Valor Final")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor final deve ser maior que zero.")]
        [DataType(DataType.Currency)]
        public decimal ValorFinal { get; set; }

        [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
        [Display(Name = "Forma de Pagamento")]
        [StringLength(45, ErrorMessage = "A forma de pagamento deve ter no máximo 45 caracteres.")]
        public string FormaPagamento { get; set; } = null!;

        [Required(ErrorMessage = "A concessionária é obrigatória.")]
        [Display(Name = "Concessionária")]
        public int IdConcessionaria { get; set; }

        [Required(ErrorMessage = "O comprador é obrigatório.")]
        [Display(Name = "Pessoa (Comprador)")]
        public int IdPessoa { get; set; }
    }
}