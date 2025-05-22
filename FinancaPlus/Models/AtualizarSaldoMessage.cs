using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancaPlus.Models
{
    public class AtualizarSaldoMessage : ValueChangedMessage<decimal>
    {
        public AtualizarSaldoMessage(decimal valor) : base(valor) { }
    }
}