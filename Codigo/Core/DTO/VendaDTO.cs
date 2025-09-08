using System;

namespace Core.DTO
{
    public class VendaDTO
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorFinal { get; set; }
        public string? FormaPagamento { get; set; }
        public string? NomeConcessionaria { get; set; }
        public string? NomePessoa { get; set; } 
    }
}