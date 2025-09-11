using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    public class AnuncioViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O veículo é obrigatório.")]
        [Display(Name = "Veículo")]
        public int IdVeiculo { get; set; }

        [Display(Name = "Venda")]
        public int IdVenda { get; set; }

        [Display(Name = "Data de Publicação")]
        public System.DateTime DataPublicacao { get; set; }

        [Display(Name = "Status do Anúncio")]
        public string? StatusAnuncio { get; set; }

        [Display(Name = "Interações")]
        public int? Interacoes { get; set; }

        [Display(Name = "Visualizações")]
        public int Visualizacoes { get; set; }

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Opcionais")]
        public string? Opcionais { get; set; }

        // Propriedades auxiliares para a view
        public IEnumerable<SelectListItem>? Veiculos { get; set; }
        public IEnumerable<SelectListItem>? Vendas { get; set; }

        // Propriedade para exibir dados do veículo relacionado
        public VeiculoViewModel? Veiculo { get; set; }
    }
}
