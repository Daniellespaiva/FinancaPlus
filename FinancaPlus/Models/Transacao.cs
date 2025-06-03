namespace FinancaPlus.Models
{
    public class Transacao
    {
        public string Descricao { get; set; } 
        public decimal Valor { get; set; }
        public string CorValor { get; set; }
        public DateTime Data { get; set; } // Adicionado para corrigir o erro
    }
}
