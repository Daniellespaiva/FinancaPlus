using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FinancaPlus.Views;

public partial class TelaCategorizacao : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private readonly TelaCategorizacaoViewModel _viewModel;
	public TelaCategorizacao()
	{
		InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new TelaCategorizacaoViewModel();
        BindingContext = _viewModel;
    }

    

    private async void ExcluirTransacao_Clicked(object sender, EventArgs e)
    {
        var transacaoSelecionada = (Transacao)((ImageButton)sender).BindingContext;
        bool confirmar = await DisplayAlert("Confirma��o", "Deseja excluir essa transa��o?", "Sim", "N�o");
        if (confirmar)
        {
            _dbHelpers.ExcluirTransacao(transacaoSelecionada);
            _viewModel.CarregarTransacoes();
        }

    }

    private async void ResetarSaldo_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirma��o", "Deseja resetar o saldo?", "Sim", "N�o");
        if (confirmar)
        {
            _dbHelpers.ResetarSaldo();
            _viewModel.AtualizarSaldo();
            await DisplayAlert("Sucesso", "Saldo resetado!", "OK");
        }

    }

    private async void ResetarDespesas_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirma��o", "Deseja resetar as despesas?", "Sim", "N�o");
        if (confirmar)
        {
            _dbHelpers.ResetarDespesas();
            _viewModel.AtualizarSaldo();
            await DisplayAlert("Sucesso", "Despesas resetadas!", "OK");
        }
    }

    private class TelaCategorizacaoViewModel
    {
        public TelaCategorizacaoViewModel()
        {
        }


        public void CarregarTransacoes()
        {
            // Implementa��o para carregar as transa��es
            // Exemplo: Atualizar uma lista de transa��es no ViewModel
        }

        public void AtualizarSaldo()
        {
            // Implementa��o para atualizar o saldo
            // Exemplo: Recalcular o saldo com base nas transa��es
        }

    }
}

public class TelaCategorizacaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Transacao> Transacoes { get; set; }
        public ObservableCollection<CategoriaDespesa> CategoriasDespesas { get; set; }

        public TelaCategorizacaoViewModel()
        {
            Transacoes = new ObservableCollection<Transacao>(); // Carregar transa��es do banco

            CategoriasDespesas = new ObservableCollection<CategoriaDespesa>
        {
            new CategoriaDespesa { Nome = "Alimenta��o", Icone = "icon_alimentacao.png" },
            new CategoriaDespesa { Nome = "Moradia", Icone = "icon_moradia.png" },
            new CategoriaDespesa { Nome = "Transporte", Icone = "icon_transporte.png" },
            new CategoriaDespesa { Nome = "Sa�de", Icone = "icon_saude.png" },
            new CategoriaDespesa { Nome = "Educa��o", Icone = "icon_educacao.png" },
            new CategoriaDespesa { Nome = "Outros", Icone = "icon_outros.png" }
        };
        }
}

    public class CategoriaDespesa
    {
        public string Nome { get; set; }
        public string Icone { get; set; }
    }


    

