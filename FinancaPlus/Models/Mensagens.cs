using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancaPlus.Models
{
    public class AtualizarFinanceiroMessage
    {
        public decimal NovaReceita { get; }
        public decimal NovaDespesa { get; }

        public AtualizarFinanceiroMessage(decimal receita, decimal despesa)
        {
            NovaReceita = receita;
            NovaDespesa = despesa;
        }

    }
}
