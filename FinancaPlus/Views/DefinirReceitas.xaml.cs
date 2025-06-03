using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinancaPlus.Views;

public partial class DefinirReceitas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private readonly DefinirReceitasViewModel _viewModel;
    private Usuario usuario; // Declaração da variável usuário
    

    private readonly List<string> _categoriasReceita = new()
    {
        "Salário", "Freelance","Investimento","Outros"
    };

    private readonly List<string> _categoriasDespesa = new()
    {
        "Moradia","Supermercado","Saúde","Educação","Entretenimento", "Roupas", "Transporte", "Outros"
    };

    public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();
    public ICommand DeleteCommand { get; private set; } = null!;

    public DefinirReceitas()
    { 
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new DefinirReceitasViewModel();
        usuario = new Usuario(); // Inicialização correta da variável
    


          BindingContext = _viewModel;

        try
        {
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());

           

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
            string nome = EntryNomeReceita.Text?.Trim();
            string categoria = PickerCategoria.SelectedItem?.ToString();
            decimal valor = decimal.TryParse(EntryValorReceita.Text, out decimal resultado) ? resultado : 0m;
            bool isReceita = RB_Receita.IsChecked;

            // Remova a exigência da descrição (nome pode ser nulo)
            if (string.IsNullOrEmpty(categoria) || valor <= 0)
            {
                await DisplayAlert("Erro", "Preencha todos os campos corretamente!", "OK");
                return;
            }

            if (isReceita)
            {
                _dbHelpers.AddReceita(new Receita { Nome = nome ?? "Sem descrição", Categoria = categoria, Valor = valor });
            }
            else
            {
                _dbHelpers.AddDespesa(new Despesa { Nome = nome ?? "Sem descrição", Categoria = categoria, Valor = valor });
            }

            // Atualizar os valores no ViewModel imediatamente
            _viewModel.ReceitaAtual = _dbHelpers.ObterTotalReceita();
            _viewModel.TotalDespesas = _dbHelpers.ObterTotalDespesas();
            _viewModel.SaldoDisponivel = _viewModel.ReceitaAtual - _viewModel.TotalDespesas;

            // Enviar mensagem para atualizar a Tela Principal
            WeakReferenceMessenger.Default.Send(new AtualizarFinanceiroMessage(
                _dbHelpers.ObterTotalReceita(),
                _dbHelpers.ObterTotalDespesas(),
                  categoria
                ));



            await DisplayAlert("Sucesso", "Transação salva!", "OK");
            await Navigation.PushAsync(new TelaPrincipal(usuario.Email));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao salvar: {ex.Message}", "OK");
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
    private decimal _receitaAtual; // Propriedade adicionada

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

    public decimal ReceitaAtual // Nova propriedade
    {
        get => _receitaAtual;
        set
        {
            _receitaAtual = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

