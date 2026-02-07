using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ModeloViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do modelo e obrigatorio.")]
        [Display(Name = "Nome do Modelo")]
        [StringLength(100, ErrorMessage = "O nome do modelo deve ter no maximo 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "A categoria e obrigatoria.")]
        [Display(Name = "Categoria")]
        public int IdCategoria { get; set; }

        // Denormalized category text persisted in legacy schema.
        public string? Categoria { get; set; }

        [Display(Name = "Versoes")]
        public string? Versoes { get; set; }

        [Required(ErrorMessage = "A marca e obrigatoria.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [Display(Name = "Marca")]
        public string? NomeMarca { get; set; }

        [Display(Name = "Categoria")]
        public string? NomeCategoria { get; set; }

        public IEnumerable<SelectListItem>? Marcas { get; set; }
    }
}
