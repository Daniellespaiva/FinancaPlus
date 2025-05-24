using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancaPlus.Models
{
    public class Categoria
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } // "Fixa" ou "Variável"
    }

}
