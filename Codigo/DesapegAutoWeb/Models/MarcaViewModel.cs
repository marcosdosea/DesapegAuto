using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class MarcaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da marca é obrigatório.")]
        [Display(Name = "Nome da Marca")]
        public string Nome { get; set; } = null!;
    }
}
