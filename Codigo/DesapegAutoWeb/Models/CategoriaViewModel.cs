using System.ComponentModel.DataAnnotations;
namespace DesapegAutoWeb.Models
{
    public class CategoriaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [Display(Name = "Nome da Categoria")]
        public string Nome { get; set; } = null!;

    }
}
