using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class VersaoViewModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da versão é obrigatório.")]
        [Display(Name = "Nome da Versão")]
        [StringLength(50, ErrorMessage = "O nome da versão deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O modelo do veículo é obrigatório.")]
        [Display(Name = "Modelo do Veículo")]
        public int IdModelo { get; set; }
    }
}