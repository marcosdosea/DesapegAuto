using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class ConcessionariaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome � obrigat�rio.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O CNPJ � obrigat�rio.")]
        public string Cnpj { get; set; } = null!;

        [Required(ErrorMessage = "O email � obrigat�rio.")]
        [EmailAddress(ErrorMessage = "Email inv�lido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O telefone � obrigat�rio.")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "A senha � obrigat�ria.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = null!;

        [Required(ErrorMessage = "O endere�o � obrigat�rio.")]
        [StringLength(80, ErrorMessage = "O endere�o deve ter no m�ximo 80 caracteres.")]
        public string Endereco { get; set; } = null!;
    }
}
