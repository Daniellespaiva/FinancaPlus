using SQLite;

namespace FinancaPlus.Models
{
    public class Gasto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }

        public decimal Valor { get; set; }

        public decimal SaldoInicial { get; set; }    
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => SaldoInicial - TotalDespesas;
        public string Categoria { get; set; } = string.Empty;
        public decimal GastoPorGateria { get; set; }   
        public DateTime Data { get; set; } = DateTime.Now;
    }
}
