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



    private async void BTN_SalvarReceita_Clicked(object sender, EventArgs e)
    {
        try
        {
            string nome = EntryNomeReceita.Text;
            string categoria = PickerCategoria.SelectedItem?.ToString();
            decimal valor = decimal.TryParse(EntryValorReceita.Text, out decimal resultado) ? resultado : 0;
            bool isReceita = RB_Receita.IsChecked;

            if (string.IsNullOrEmpty(nome) || categoria == null || valor <= 0)
            {
                await DisplayAlert("Erro", "Preencha todos os campos corretamente!", "OK");
                return;
            }

            if (isReceita)
            {
                _dbHelpers.AddReceita(new Receita { Nome = nome, Categoria = categoria, Valor = valor });
            }
            else
            {
                _dbHelpers.AddDespesa(new Despesa { Nome = nome, Categoria = categoria, Valor = valor });
            }

            // Enviar mensagem para atualizar saldo na tela Minhas Finanças
            WeakReferenceMessenger.Default.Send(new AtualizarSaldoMessage(valor));

            await DisplayAlert("Sucesso", "Informação adicionada!", "OK");

            // Navegar para a página Minhas Finanças
            await Navigation.PushAsync(new MinhaFinancas());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao salvar: {ex.Message}", "OK");
        }
    }

    private void IrParaPerfil_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaRelatorios_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaConfig_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaMinhasFinancas_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaTelaInicial_Clicked(object sender, EventArgs e)
    {

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

