using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancaPlus.Models
{
    public class AtualizarSaldoMessage : ValueChangedMessage<decimal>
    {
        public AtualizarSaldoMessage(decimal valor) : base(valor) { }

        // Certifique-se de que a classe tem uma propriedade pública acessível
        public decimal Saldo => Value;
    }

}