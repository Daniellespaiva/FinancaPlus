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

    private async void BTN_ExcluirMetaPorCategoria_Clicked(object sender, EventArgs e)
    {
        try
        {
            string categoriaSelecionada = PickerCategoria.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(categoriaSelecionada))
            {
                await DisplayAlert("Erro", "Selecione uma categoria para excluir!", "OK");
                return;
            }

            _viewModel.ExcluirMetaPorCategoria(categoriaSelecionada);
            await DisplayAlert("Sucesso", "Metas excluídas com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao excluir meta: {ex.Message}", "OK");
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

    public void ExcluirMetaPorCategoria(string categoria)
    {
        var metasParaExcluir = _dbHelpers.GetMetas().Where(m => m.Categoria == categoria).ToList();
        
        foreach (var meta in metasParaExcluir)
        {
            _dbHelpers.DeleteMetasPorCategoria(meta.Categoria); // Corrigido para usar o método correto
        }

        CarregarMetas(); // Atualiza a lista após a exclusão
    }
}
