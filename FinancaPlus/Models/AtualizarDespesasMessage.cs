using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancaPlus.Models
{
    public class AtualizarDespesasMessage : ValueChangedMessage<decimal>
    {
        public AtualizarDespesasMessage(decimal valor) : base(valor) { }
    }
}
