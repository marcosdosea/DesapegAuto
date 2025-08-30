using System;
using System.Collections.Generic;

namespace Core.DTO
{
    public class ModeloDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Versoes { get; set; }
        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }
    }
}