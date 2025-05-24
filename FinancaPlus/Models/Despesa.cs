using SQLite;

namespace FinancaPlus.Models
{
    public class Despesa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Categoria { get; set; }
        public DateTime Data { get; set; }
    }
}




