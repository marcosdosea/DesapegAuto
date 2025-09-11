using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class AnuncioViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ve�culo � obrigat�rio.")]
        [Display(Name = "Ve�culo")]
        public int IdVeiculo { get; set; }

        [Display(Name = "Venda")]
        public int IdVenda { get; set; }

        [Display(Name = "Data de Publica��o")]
        public System.DateTime DataPublicacao { get; set; }

        [Display(Name = "Status do An�ncio")]
        public string? StatusAnuncio { get; set; }

        [Display(Name = "Intera��es")]
        public int? Interacoes { get; set; }

        [Display(Name = "Visualiza��es")]
        public int Visualizacoes { get; set; }

        [Display(Name = "Descri��o")]
        public string? Descricao { get; set; }

        [Display(Name = "Opcionais")]
        public string? Opcionais { get; set; }

        // Propriedades auxiliares para a view
        public IEnumerable<SelectListItem>? Veiculos { get; set; }
        public IEnumerable<SelectListItem>? Vendas { get; set; }

        // Propriedade para exibir dados do ve�culo relacionado
        public VeiculoViewModel? Veiculo { get; set; }
    }
}
