using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancaPlus.Models
{
    public class CategoriaDespesa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; } // Exemplo: "Fixa" ou "Variável"
        public string Icone { get; set; }
        public decimal ValorGasto { get; set; } // Adicionado a propriedade ValorGasto
    }
}

