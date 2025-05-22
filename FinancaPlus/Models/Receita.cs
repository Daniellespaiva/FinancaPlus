using SQLite;

namespace FinancaPlus.Models
{
    public class Receita
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
