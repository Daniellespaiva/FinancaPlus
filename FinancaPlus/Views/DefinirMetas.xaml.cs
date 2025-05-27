using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinancaPlus.Views;

public partial class DefinirMetas : ContentPage
{
    private readonly DefinirMetasViewModel _viewModel;

    public DefinirMetas()
    {
        InitializeComponent();
        _viewModel = new DefinirMetasViewModel();
        BindingContext = _viewModel;
    }

    private async void BTN_AdicionarMeta_Clicked(object sender, EventArgs e)
    {
        try
        {
            string nome = EntryNomeMeta.Text?.Trim();
            string categoria = PickerCategoria.SelectedItem?.ToString();
            decimal valor = decimal.TryParse(EntryValorMeta.Text, out decimal resultado) ? resultado : 0m;
            DateTime dataConclusao = PickerDataMeta.Date;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(categoria) || valor <= 0)
            {
                await DisplayAlert("Erro", "Preencha todos os campos corretamente!", "OK");
                return;
            }

            _viewModel.AdicionarMeta(nome, valor, categoria, dataConclusao);
            await DisplayAlert("Sucesso", "Meta adicionada!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao adicionar meta: {ex.Message}", "OK");
        }
    }

    private async void BTN_ApagarHistoricoCategoria_Clicked(object sender, EventArgs e)
    {
        try
        {
            string categoriaSelecionada = PickerCategoriaExcluir.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(categoriaSelecionada))
            {
                await DisplayAlert("Erro", "Selecione uma categoria para excluir!", "OK");
                return;
            }

            _viewModel.ApagarMetaPorCategoria(categoriaSelecionada);
            await DisplayAlert("Sucesso", "Metas excluídas com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao excluir metas: {ex.Message}", "OK");
        }
    }

    private async void BTN_ResetarSaldoCategoria_Clicked(object sender, EventArgs e)
    {
        try
        {
            string categoriaSelecionada = PickerCategoriaReset.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(categoriaSelecionada))
            {
                await DisplayAlert("Erro", "Selecione uma categoria para resetar o saldo!", "OK");
                return;
            }

            _viewModel.ResetarSaldoPorCategoria(categoriaSelecionada);
            await DisplayAlert("Sucesso", "Saldo resetado com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao resetar saldo: {ex.Message}", "OK");
        }

    }

}
public partial class DefinirMetasViewModel : INotifyPropertyChanged
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private ObservableCollection<Meta> _metasDefinidas;

    public ObservableCollection<Meta> MetasDefinidas
    {
        get => _metasDefinidas;
        set
        {
            _metasDefinidas = value;
            OnPropertyChanged(nameof(MetasDefinidas));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DefinirMetasViewModel()
    {
        _dbHelpers = new SQLiteDatabaseHelpers();
        CarregarMetas();
    }

    public void CarregarMetas()
    {
        MetasDefinidas = new ObservableCollection<Meta>(_dbHelpers.GetMetas());
    }

    public void AdicionarMeta(string nome, decimal valor, string categoria, DateTime dataConclusao)
    {
        var novaMeta = new Meta { Nome = nome, Valor = valor, Categoria = categoria, DataConclusao = dataConclusao };
        _dbHelpers.AddMeta(novaMeta);
        CarregarMetas();
    }

    public void ApagarMetaPorCategoria(string categoria)
    {
        _dbHelpers.DeleteMetasPorCategoria(categoria);
        CarregarMetas();
    }

    public void ResetarSaldoPorCategoria(string categoria)
    {
        _dbHelpers.ResetarSaldoPorCategoria(categoria);
    }

}
