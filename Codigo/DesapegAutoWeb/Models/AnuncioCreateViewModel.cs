using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class AnuncioCreateViewModel
    {
        [Required(ErrorMessage = "A concessionaria e obrigatoria.")]
        [Display(Name = "Concessionaria")]
        public int IdConcessionaria { get; set; }

        [Required(ErrorMessage = "A marca e obrigatoria.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "O modelo e obrigatorio.")]
        [Display(Name = "Modelo")]
        public int IdModelo { get; set; }

        [Required(ErrorMessage = "O ano e obrigatorio.")]
        [Range(1886, 2200, ErrorMessage = "O ano deve ser valido.")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "A cor e obrigatoria.")]
        public string Cor { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quilometragem e obrigatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quilometragem nao pode ser negativa.")]
        public int Quilometragem { get; set; }

        [Required(ErrorMessage = "O preco e obrigatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preco deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A placa e obrigatoria.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter 7 caracteres.")]
        public string Placa { get; set; } = string.Empty;

        [Display(Name = "Descricao")]
        public string? Descricao { get; set; }

        public List<string> OpcionaisSelecionados { get; set; } = new();
    }
}
