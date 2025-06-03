using CommunityToolkit.Mvvm.Messaging;
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

        // Atualiza automaticamente quando há mudança nos dados financeiros
        WeakReferenceMessenger.Default.Register<AtualizarFinanceiroMessage>(this, (recipient, message) =>
        {
            if (recipient is TelaCategorizacao telaCategorizacao)
            {
                telaCategorizacao._viewModel.AtualizarGastosPorCategoria();
                telaCategorizacao._viewModel.AtualizarCategoriasDespesas();
                telaCategorizacao._viewModel.AtualizarSaldo();
            }
        });

        IniciarAtualizacaoAutomatica(); // Começa a atualização automática
    }


    private async void IniciarAtualizacaoAutomatica()
    {
        while (true)
        {
            _viewModel.AtualizarGastosPorCategoria();
            _viewModel.AtualizarCategoriasDespesas();
            _viewModel.AtualizarSaldo();
            await Task.Delay(1800000); // Aguarda 30 minutos antes da próxima atualização
        }
    }



    private async void ExcluirTransacao_Clicked(object sender, EventArgs e)
    {
        var transacaoSelecionada = (Transacao)((ImageButton)sender).BindingContext;
        bool confirmar = await DisplayAlert("Confirmação", "Deseja excluir essa transação?", "Sim", "Não");
        if (confirmar)
        {
            _dbHelpers.ExcluirTransacao(transacaoSelecionada);
            _viewModel.AtualizarTransacoesRecentes(); // Substituído CarregarTransacoes por AtualizarTransacoesRecentes
        }

    }

    private async void ResetarSaldo_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmação", "Deseja resetar o saldo?", "Sim", "Não");
        if (confirmar)
        {
            _dbHelpers.ResetarSaldo();
            _viewModel.AtualizarSaldo();
            await DisplayAlert("Sucesso", "Saldo resetado!", "OK");
        }

    }

    private async void ResetarDespesas_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmação", "Deseja resetar as despesas?", "Sim", "Não");
        if (confirmar)
        {
            _dbHelpers.ResetarDespesas();
            _viewModel.AtualizarSaldo();
            await DisplayAlert("Sucesso", "Despesas resetadas!", "OK");
        }
    }

    public class TelaCategorizacaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly SQLiteDatabaseHelpers _dbHelpers;

        public ObservableCollection<Transacao> Transacoes { get; set; }
        public ObservableCollection<CategoriaDespesa> CategoriasDespesas { get; set; }
        public decimal Saldo { get; private set; } // Adicionando propriedade Saldo

        public TelaCategorizacaoViewModel()
        {
            _dbHelpers = new SQLiteDatabaseHelpers();
            Transacoes = new ObservableCollection<Transacao>();
            CategoriasDespesas = new ObservableCollection<CategoriaDespesa>();

            AtualizarCategoriasDespesas();
            AtualizarTransacoesRecentes();
            AtualizarSaldo(); // Inicializa o saldo
        }

        public void AtualizarSaldo()
        {
            Saldo = _dbHelpers.ObterTotalReceita() - _dbHelpers.ObterTotalDespesas(); // Corrigido cálculo do saldo
            OnPropertyChanged(nameof(Saldo));
        }

        public void AtualizarCategoriasDespesas()
        {
            var categoriasFixas = _dbHelpers.GetCategoriasFixas(); // Removido o argumento incorreto
            var categoriasVariaveis = _dbHelpers.GetCategoriasVariaveis(); // Removido o argumento incorreto

            CategoriasDespesas.Clear(); // Limpa a lista antes de adicionar novas categorias

            foreach (var categoria in categoriasFixas)
            {
                CategoriasDespesas.Add(new CategoriaDespesa
                {
                    Nome = categoria.Nome,
                    ValorGasto = categoria.ValorGasto,
                    Icone = categoria.Icone
                });
            }

            foreach (var categoria in categoriasVariaveis)
            {
                CategoriasDespesas.Add(new CategoriaDespesa
                {
                    Nome = categoria.Nome,
                    ValorGasto = categoria.ValorGasto,
                    Icone = categoria.Icone
                });
            }
        }


        public void AtualizarTransacoesRecentes()
        {
            var transacoes = _dbHelpers.ObterTransacoesRecentes();
            Transacoes.Clear();

            foreach (var transacao in transacoes)
            {
                Transacoes.Add(new Transacao
                {
                    Descricao = transacao.Descricao,
                    Valor = transacao.Valor,
                    CorValor = transacao.Valor < 0 ? "Red" : "Green"
                });
            }

            OnPropertyChanged(nameof(Transacoes));
        }

        public void AtualizarGastosPorCategoria()
        {
            var despesasPorCategoria = _dbHelpers.ObterDespesasPorCategoria();

            CategoriasDespesas.Clear();
            foreach (var categoria in despesasPorCategoria)
            {
                CategoriasDespesas.Add(new CategoriaDespesa
                {
                    Nome = categoria.Key,
                    Icone = DefinirIconePorCategoria(categoria.Key),
                    ValorGasto = categoria.Value
                });
            }

            OnPropertyChanged(nameof(CategoriasDespesas));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string DefinirIconePorCategoria(string categoria)
        {
            return categoria switch
            {
                "Alimentação" => "icon_alimentacao.png",
                "Moradia" => "icon_moradia.png",
                "Transporte" => "icon_transporte.png",
                "Saúde" => "icon_saude.png",
                "Educação" => "icon_educacao.png",
                "Outros" => "icon_outros.png",
                _ => "icon_padrao.png"
            };
        }
    }

    public class CategoriaDespesa
    {
        public string Nome { get; set; }
        public string Icone { get; set; }
        public decimal ValorGasto { get; set; }
    }

    private async void IrParaRelatorios_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GerarRelatorio()); // Abre tela de relatórios
    }

    private void IrParaConfig_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ConfiguracaoPage());
    }



    private async void IrParaTelaInicial_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TelaPrincipal("email@exemplo.com")); // Abre a tela principal
    }
}





