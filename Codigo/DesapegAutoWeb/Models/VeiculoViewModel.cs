using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class VeiculoViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A concessionária é obrigatória.")]
        [Display(Name = "Concessionária")]
        public int IdConcessionaria { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        [Display(Name = "Modelo")]
        public int IdModelo { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório.")]
        [Display(Name = "Ano de Fabricação")]
        [Range(1900, 2100, ErrorMessage = "O ano deve ser válido.")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória.")]
        [Display(Name = "Cor")]
        public string Cor { get; set; } = null!;

        [Required(ErrorMessage = "A quilometragem é obrigatória.")]
        [Display(Name = "Quilometragem")]
        [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser um valor negativo.")]
        public int Quilometragem { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter 7 caracteres.")]
        public string Placa { get; set; } = null!;

        /// <summary>
        /// Status do Veículo. Ex: D (Disponível), V (Vendido), P (Pendente), I (Indisponível)
        /// </summary>
        [Display(Name = "Status do Veículo")]
        public string? Status { get; set; }


        // --- Propriedades "Ajudantes" que só existem aqui para a TELA ---

        // Para mostrar o nome da Marca e do Modelo na tela de listagem
        public string? NomeMarca { get; set; }
        public string? NomeModelo { get; set; }

        // Para popular os dropdowns (<select>) no formulário de edição
        public IEnumerable<SelectListItem>? Marcas { get; set; }
        public IEnumerable<SelectListItem>? Modelos { get; set; }
    }
}