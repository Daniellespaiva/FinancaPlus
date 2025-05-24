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
    private readonly DefinirReceitasViewModel _viewModel;

    private readonly List<string> _categoriasReceita = new()
    {
        "Salario", "Freelance","Investimento","Outros"
    };

    private readonly List<string> _categoriasDespesa = new()
    {
        "Moradia","Supermercado","Saúde","Educação","Entretenimento", "Roupas", "Transporte", "Outros"
    };

    public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();
    public ICommand DeleteCommand { get; private set; } = null!;

    public DefinirReceitas()
    {
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new DefinirReceitasViewModel();
        BindingContext = _viewModel;

        try
        {
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());

            InitializeComponent();

            EntryNomeReceita = this.FindByName<Entry>("EntryNomeReceita") ?? throw new Exception("EntryNomeReceita não encontrado.");
            EntryValorReceita = this.FindByName<Entry>("EntryValorReceita") ?? throw new Exception("EntryValorReceita não encontrado.");
            PickerCategoria = this.FindByName<Picker>("PickerCategoria") ?? throw new Exception("PickerCategoria não encontrado.");



            // Inicializa Picker com categorias de receita
            PickerCategoria.ItemsSource = _categoriasReceita;
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro na inicialização: {ex.Message}", "OK");
        }
    }

    private void Tipo_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (RB_Receita.IsChecked)
        {
            PickerCategoria.ItemsSource = _categoriasReceita;
        }
        else if (RB_Despesa.IsChecked)
        {
            PickerCategoria.ItemsSource = _categoriasDespesa;
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

    private async void BTN_SalvarReceita_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TelaPrincipal("example@example.com"));
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



    
    
public class DefinirReceitasViewModel : INotifyPropertyChanged
{
    private decimal _saldoDisponivel;
    private decimal _totalDespesas;

    public decimal SaldoDisponivel
    {
        get => _saldoDisponivel;
        set
        {
            _saldoDisponivel = value;
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

