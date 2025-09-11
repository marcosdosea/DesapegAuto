using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ModeloViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do modelo � obrigat�rio.")]
        [Display(Name = "Nome do Modelo")]
        [StringLength(100, ErrorMessage = "O nome do modelo deve ter no m�ximo 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "A categoria � obrigat�ria.")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; } = null!;

        [Display(Name = "Vers�es")]
        public string? Versoes { get; set; }

        [Required(ErrorMessage = "A marca � obrigat�ria.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        // Helper properties for UI
        [Display(Name = "Marca")]
        public string? NomeMarca { get; set; }

        // Dropdown list to populate marcas in forms
        public IEnumerable<SelectListItem>? Marcas { get; set; }
    }
}
