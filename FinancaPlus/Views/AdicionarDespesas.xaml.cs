using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FinancaPlus.Views;

public partial class AdicionarDespesas : ContentPage
{
    private readonly AdicionarDespesaViewModel _viewModel;
    private readonly SQLiteDatabaseHelpers _dbHelpers;

    public AdicionarDespesas()
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new AdicionarDespesaViewModel();

        BindingContext = _viewModel; // Define o ViewModel como contexto da tela

        // Carregar despesas do banco de dados
        _viewModel.ListaDespesas = new ObservableCollection<Despesa>(_dbHelpers.GetDespesas()); new ObservableCollection<Receita>();

        DespesaListView.ItemsSource = _viewModel.ListaDespesas; // Define a fonte de dados do ListView
    }

    private async void BTN_AdicionarDespesa_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_viewModel.NomeDespesa) || _viewModel.ValorDespesa <= 0 || string.IsNullOrWhiteSpace(_viewModel.CategoriaSelecionada))
            {
                await DisplayAlert("Erro", "Preencha todos os campos!", "Fechar");
                return;
            }

            var novaDespesa = new Despesa
            {
                Nome = _viewModel.NomeDespesa,
                Categoria = _viewModel.CategoriaSelecionada,
                Valor = _viewModel.ValorDespesa,
                Data = _viewModel.DataDespesa
            };

            _dbHelpers.AddDespesa(novaDespesa); // Salvar no banco de dados
            _viewModel.ListaDespesas.Add(novaDespesa); // Atualiza UI automaticamente

            // **Envia mensagem para atualizar saldo na Tela Principal**
            WeakReferenceMessenger.Default.Send(new AtualizarSaldoMessage(novaDespesa.Valor));

            await DisplayAlert("Sucesso", $"Despesa '{novaDespesa.Nome}' adicionada!", "OK");

            // Limpa os campos após adicionar
            _viewModel.NomeDespesa = string.Empty;
            _viewModel.ValorDespesa = 0m;
            _viewModel.CategoriaSelecionada = null;

            await Navigation.PushAsync(new TelaPrincipal("example@example.com"));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao adicionar a despesa: {ex.Message}", "OK");
        }
    }  


    // Crie uma classe para encapsular a mensagem
    public class AtualizarSaldoMessage
    {
        public decimal Valor { get; }

        public AtualizarSaldoMessage(decimal valor)
        {
            Valor = valor;
        }
    }

    private async void BTN_ResetarSaldoCategoria_Clicked(object sender, EventArgs e)
    {

        if (PickerCategoriaReset.SelectedItem == null)
        {
            await DisplayAlert("Erro", "Selecione uma categoria para resetar saldo!", "OK");
            return;
        }

        string categoriaSelecionada = PickerCategoriaReset.SelectedItem.ToString();

        bool confirmacao = await DisplayAlert("Confirmação", $"Deseja realmente resetar o saldo da categoria '{categoriaSelecionada}'?", "Sim", "Cancelar");
        if (!confirmacao) return;

        _dbHelpers.ResetarSaldoPorCategoria(categoriaSelecionada);

        await DisplayAlert("Sucesso", $"Saldo da categoria '{categoriaSelecionada}' resetado para R$ 0,00!", "OK");
    }

    private async void BTN_ApagarHistoricoCategoria_Clicked(object sender, EventArgs e)
    {

        if (PickerCategoriaExcluir.SelectedItem == null)
        {
            await DisplayAlert("Erro", "Selecione uma categoria para excluir!", "OK");
            return;
        }

        string categoriaSelecionada = PickerCategoriaExcluir.SelectedItem.ToString();

        bool confirmacao = await DisplayAlert("Confirmação", $"Deseja apagar todas as receitas da categoria '{categoriaSelecionada}'?", "Sim", "Cancelar");
        if (!confirmacao) return;

        _dbHelpers.DeleteReceitasPorCategoria(categoriaSelecionada);

        _viewModel.ListaDespesas = new ObservableCollection<Despesa>(_dbHelpers.GetDespesas()); // Atualiza a lista exibida
        DespesaListView.ItemsSource = _viewModel.ListaDespesas; // Atualiza o ListView na interface

        await DisplayAlert("Sucesso", $"Despesa da categoria '{categoriaSelecionada}' apagadas!", "OK");
    }
}


public class AdicionarDespesaViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _nomeDespesa;
    private decimal _valorDespesa;
    private string _categoriaSelecionada;
    private DateTime _dataDespesa = DateTime.Now;

    public string NomeDespesa
    {
        get => _nomeDespesa;
        set
        {
            _nomeDespesa = value;
            OnPropertyChanged(nameof(NomeDespesa));
        }
    }

    public decimal ValorDespesa
    {
        get => _valorDespesa;
        set
        {
            _valorDespesa = value;
            OnPropertyChanged(nameof(ValorDespesa));
        }
    }

    public string CategoriaSelecionada
    {
        get => _categoriaSelecionada;
        set
        {
            _categoriaSelecionada = value;
            OnPropertyChanged(nameof(CategoriaSelecionada));
        }
    }

    public DateTime DataDespesa
    {
        get => _dataDespesa;
        set
        {
            _dataDespesa = value;
            OnPropertyChanged(nameof(DataDespesa));
        }
    }

    public ObservableCollection<Despesa> ListaDespesas { get; set; } = new ObservableCollection<Despesa>();

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
