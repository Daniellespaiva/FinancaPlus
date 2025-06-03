using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancaPlus.Models
{
    public class AtualizarFinanceiroMessage
    {
        public decimal NovaReceita { get; }
        public decimal NovaDespesa { get; }
        public string Categoria { get; }

        public AtualizarFinanceiroMessage(decimal novaReceita, decimal novaDespesa, string categoria)
        {
            NovaReceita = novaReceita;
            NovaDespesa = novaDespesa;
            Categoria = categoria;
        }
    }
}

