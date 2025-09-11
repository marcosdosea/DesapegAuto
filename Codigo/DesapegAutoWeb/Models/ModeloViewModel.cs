using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ModeloViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do modelo é obrigatório.")]
        [Display(Name = "Nome do Modelo")]
        [StringLength(100, ErrorMessage = "O nome do modelo deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; } = null!;

        [Display(Name = "Versões")]
        public string? Versoes { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        // Helper properties for UI
        [Display(Name = "Marca")]
        public string? NomeMarca { get; set; }

        // Dropdown list to populate marcas in forms
        public IEnumerable<SelectListItem>? Marcas { get; set; }
    }
}
