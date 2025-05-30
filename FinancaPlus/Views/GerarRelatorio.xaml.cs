using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FinancaPlus.Views;

public partial class GerarRelatorio : ContentPage
{
    private readonly GerarRelatorioViewModel _viewModel;

    public GerarRelatorio()
    {
        InitializeComponent();
        _viewModel = new GerarRelatorioViewModel();
        BindingContext = _viewModel;
    }

    private async void BTN_GerarRelatorio_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Relatório", "O relatório foi gerado com sucesso!", "OK");
        // Aqui você pode adicionar a lógica para buscar dados e atualizar o gráfico
    }

    private async void BTN_ResetarDados_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmação", "Deseja resetar os dados do relatório?", "Sim", "Não");
        if (confirmar)
        {
            _viewModel.Transacoes.Clear();
            await DisplayAlert("Sucesso", "Dados do relatório resetados!", "OK");
        }
    }

    private class GerarRelatorioViewModel
    {
        public ObservableCollection<Transacao> Transacoes { get; set; }

        public GerarRelatorioViewModel()
        {
            Transacoes = new ObservableCollection<Transacao>();
        }
    }
}

public class GerarRelatorioViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<CategoriaDespesa> CategoriasDespesas { get; set; }
    public ObservableCollection<Transacao> Transacoes { get; set; }

    public GerarRelatorioViewModel()
    {
        Transacoes = new ObservableCollection<Transacao>(); // Carregar transações do banco

        CategoriasDespesas = new ObservableCollection<CategoriaDespesa>
        {
            new CategoriaDespesa { Nome = "Alimentação", Icone = "icon_alimentacao.png" },
            new CategoriaDespesa { Nome = "Moradia", Icone = "icon_moradia.png" },
            new CategoriaDespesa { Nome = "Transporte", Icone = "icon_transporte.png" },
            new CategoriaDespesa { Nome = "Saúde", Icone = "icon_saude.png" },
            new CategoriaDespesa { Nome = "Educação", Icone = "icon_educacao.png" },
            new CategoriaDespesa { Nome = "Outros", Icone = "icon_outros.png" }
        };
    }
}

public class Categoria
{
    public string Nome { get; set; }
    public string Icone { get; set; }
}