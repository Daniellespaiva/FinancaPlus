using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinancaPlus.Views;

public partial class DefinirReceitas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private readonly DefinirReceitasViewModel _viewModel; // Adiciona o ViewModel como um campo privado

    public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();

    public ICommand DeleteCommand { get; private set; } = null!;

    public DefinirReceitas()
    {
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new DefinirReceitasViewModel(); // Inicializa o ViewModel

        BindingContext = _viewModel; // Define o BindingContext para o ViewModel


        try
        {
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());
            InitializeComponent();

            EntryNomeReceita = this.FindByName<Entry>("EntryNomeReceita");
            EntryValorReceita = this.FindByName<Entry>("EntryValorReceita");
            PickerCategoria = this.FindByName<Picker>("PickerCategoria");


            if (ReceitasListView != null)
            {
                ReceitasListView.ItemsSource = ListaReceitas;
            }
            else
            {
                throw new Exception("Erro ao encontrar ListView 'ReceitasListView'. Verifique o nome no XAML.");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro na inicialização: {ex.Message}", "OK");
        }
    }

    private async void BTN_AdicionarReceita_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EntryNomeReceita.Text) || string.IsNullOrWhiteSpace(EntryValorReceita.Text) || PickerCategoria.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Preencha todos os campos!", "Fechar");
                return;
            }

            if (!decimal.TryParse(EntryValorReceita.Text, out decimal valorReceita))
            {
                await DisplayAlert("Erro", "Insira um valor válido!", "Fechar");
                return;
            }

            var categoriaSelecionada = PickerCategoria.SelectedItem?.ToString();
            if (categoriaSelecionada == null)
            {
                await DisplayAlert("Erro", "Selecione uma categoria válida!", "Fechar");
                return;
            }

            var novaReceita = new Receita
            {
                Nome = EntryNomeReceita.Text,
                Valor = valorReceita,
                Categoria = categoriaSelecionada
            };

            _dbHelpers.AddReceita(novaReceita);
            ListaReceitas.Add(novaReceita);

            // **Envia mensagem para atualizar o saldo na TelaPrincipal**
            WeakReferenceMessenger.Default.Send(new AtualizarSaldoMessage(valorReceita));
            await DisplayAlert("Sucesso", $"Receita '{novaReceita.Nome}' adicionada!", "OK");

            EntryNomeReceita.Text = string.Empty;
            EntryValorReceita.Text = string.Empty;
            PickerCategoria.SelectedItem = null;

            await Navigation.PushAsync(new TelaPrincipal("example@example.com"));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao adicionar a receita: {ex.Message}", "OK");
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

        ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas()); // Atualiza a lista exibida
        ReceitasListView.ItemsSource = ListaReceitas; // Atualiza o ListView na interface

        await DisplayAlert("Sucesso", $"Receitas da categoria '{categoriaSelecionada}' apagadas!", "OK");
    }
}
public class DefinirReceitasViewModel : INotifyPropertyChanged
{
    private decimal _saldoInicial;
    private decimal _totalDespesas;

    public decimal SaldoInicial
    {
        get => _saldoInicial;
        set
        {
            _saldoInicial = value;
            OnPropertyChanged();
        }
    }

    public decimal TotalDespesas
    {
        get => _totalDespesas;
        set
        {
            _totalDespesas = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

