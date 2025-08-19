using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class VeiculoViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdConcessionaria { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        [Display(Name = "Modelo")]
        public int IdModelo { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório.")]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória.")]
        [Display(Name = "Cor")]
        public string Cor { get; set; } = null!;

        [Required(ErrorMessage = "A quilometragem é obrigatória.")]
        public int Quilometragem { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória.")]
        [StringLength(7)]
        public string Placa { get; set; } = null!;


        // --- Propriedades "Ajudantes" que só existem aqui para a TELA ---

        // Para mostrar o nome da Marca e do Modelo na tela de listagem
        public string? NomeMarca { get; set; }
        public string? NomeModelo { get; set; }

        // Para popular os dropdowns (<select>) no formulário de edição
        public IEnumerable<SelectListItem>? Marcas { get; set; }
        public IEnumerable<SelectListItem>? Modelos { get; set; }
    }
}