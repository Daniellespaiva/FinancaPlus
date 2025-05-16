using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancaPlus.Models
{
    public class Gasto
    {
        public decimal SaldoInicial { get; set; }    
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => SaldoInicial - TotalDespesas;
        public string Categoria { get; set; } = string.Empty;
        public decimal GastoPorGateria { get; set; }    
    }
}
