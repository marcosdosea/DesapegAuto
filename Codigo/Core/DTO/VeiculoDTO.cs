using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class VeiculoDTO
    {
        public int Id { get; set; }
        public int Concessionaria { get; set; }
        public int Ano { get; set; }
        public string? Cor { get; set; }
        public int Quilometragem { get; set; }
        public decimal Preco { get; set; }
        public string? Placa { get; set; }
    }
}
