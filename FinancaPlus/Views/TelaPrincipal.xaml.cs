using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;


namespace FinancaPlus.Views;
public partial class TelaPrincipal : ContentPage
{
    private readonly TelaPrincipalViewModel _viewModel;
    private readonly SQLiteDatabaseHelpers _dbHelpers;


    public TelaPrincipal(string email)
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new TelaPrincipalViewModel(email);

        _viewModel.UsuarioLogado = _dbHelpers.GetUsuario(email) ?? new Usuario();
        _viewModel.SaldoDisponivel = decimal.TryParse(Preferences.Get("SaldoDisponivel", "0"), out decimal saldo) ? saldo : 0m;
        _viewModel.NomeUsuario = "Usuário Teste"; // Valor de teste
        _viewModel.DataAtual = DateTime.Now;
        _viewModel.HoraAtual = DateTime.Now;


        BindingContext = _viewModel; // Define corretamente a Binding


        BindingContext = _viewModel;


        // Certifique-se de que os nomes das Labels correspondem aos IDs definidos no arquivo XAML
        LBL_NomeUsuario = this.FindByName<Label>("LBL_NomeUsuario");
        LBL_Saldo = this.FindByName<Label>("LBL_SaldoDisponivel"); // Adiciona a referência correta para LBL_Saldo
        LBL_Despesas = this.FindByName<Label>("LBL_TotalDespesas"); // Adiciona a referência correta para LBL_Despesas
        LBL_Receita = this.FindByName<Label>("LBL_Receita");
        CarregarNomeUsuario();
        // **Atualiza a Label automaticamente quando o saldo for modificado**
        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(_viewModel.SaldoDisponivel))
            {
                LBL_Saldo.Text = $"Saldo: R$ {_viewModel.SaldoDisponivel:N2}";
            }
            else if (e.PropertyName == nameof(_viewModel.TotalDespesas))
            {
                LBL_Despesas.Text = $"Despesas: R$ {_viewModel.TotalDespesas:N2}";
            }
        };

       
    }
    private void CarregarNomeUsuario()
    {
        if (LBL_NomeUsuario != null && _viewModel.UsuarioLogado != null)
        {
            LBL_NomeUsuario.Text = $"Bem-vindo, {_viewModel.UsuarioLogado.Nome}!";
        }
        else
        {
            Debug.WriteLine("Erro: Nome do usuário não pôde ser carregado.");
        }
    }





    private async void IrParaPerfil_Clicked(object sender, EventArgs e)
    {
        string emailUsuario = _viewModel.UsuarioLogado?.Email ?? "email@exemplo.com";
        await Navigation.PushAsync(new Perfil(emailUsuario));
    }

    private void IrParaConfig_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ConfiguracaoPage());
    }

    private async void IrParaTelaPrincipal_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TelaPrincipal("email@exemplo.com")); // Abre a tela principal
    }



    private async void IrParaRelatorios_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GerarRelatorio()); // Abre tela de relatórios
    }

    private async void IrParaMetas_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DefinirMetas()); // Abre metas
    }

    private async void IrParaLogout_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync(); // Faz logout e retorna à tela inicial
    }

    private async void BTN_receitas_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new DefinirReceitas());

        }
        catch
        {

        }
    }

    


    private async void BTN_definirMetas_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new DefinirMetas());
        }
        catch
        {

        }
    }

    private async void BTN_relatorios_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new GerarRelatorio());

        }
        catch
        {

        }
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {
        Navigation.PopToRootAsync();
    }

    private async void BTN_Categorias_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new CategoriasPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao abrir a tela de despesas: {ex.Message}", "OK");
        }
    }
}

public partial class TelaPrincipalViewModel : INotifyPropertyChanged
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
     private decimal _saldoDisponivel = 0m ;
    private decimal _receitaAtual = 0m;
    private decimal _totalDespesas = 0m ;
    private DateTime _dataAtual;
    private DateTime _horaAtual;
   



    public event PropertyChangedEventHandler? PropertyChanged;
    // Binding do nome do usuário
    private string _nomeUsuario;
    public string NomeUsuario
    {
        get => _nomeUsuario;
        set
        {
            _nomeUsuario = value;
            OnPropertyChanged(nameof(NomeUsuario));
        }
    }

    public DateTime DataAtual
    {
        get => _dataAtual;
        set
        {
            _dataAtual = value;
            OnPropertyChanged(nameof(DataAtual));
        }
    }

    public DateTime HoraAtual
    {
        get => _horaAtual;
        set
        {
            _horaAtual = value;
            OnPropertyChanged(nameof(HoraAtual));
        }
    }



    // Binding do saldo disponível
    public decimal SaldoDisponivel
    {
        get => _saldoDisponivel;
        set
        {
            _saldoDisponivel = value;
            OnPropertyChanged(nameof(SaldoDisponivel));
            OnPropertyChanged(nameof(Saldo));
        }
    }

    // Binding da receita atual
    public decimal ReceitaAtual
    {
        get => _receitaAtual;
        set
        {
            _receitaAtual = value;
            OnPropertyChanged(nameof(ReceitaAtual));
        }
    }

    // Binding das despesas
    public decimal TotalDespesas
    {
        get => _totalDespesas;
        set
        {
            _totalDespesas = value;
            OnPropertyChanged(nameof(TotalDespesas));
            OnPropertyChanged(nameof(Saldo)); // Atualiza automaticamente
        }
    }

    // Saldo calculado dinamicamente
    public decimal Saldo => SaldoDisponivel - TotalDespesas;






    // Propriedade não anulável inicializada no construtor
    public Usuario UsuarioLogado { get; set; } = new Usuario();
    public Models.Despesa Gasto { get; set; } = new Models.Despesa(); // Inicializa os gastos
    public Dictionary<string, float> GastosPorCategoria { get; set; }


    public TelaPrincipalViewModel(string email)
    {

        _dbHelpers = new SQLiteDatabaseHelpers();
        UsuarioLogado = _dbHelpers.GetUsuario(email) ?? new Usuario
        {
            Nome = string.Empty,
            Email = string.Empty
        };
        SaldoDisponivel = ObterSaldoDoBancoDeDados();
        ReceitaAtual = ObterTotalDespesas();
        GastosPorCategoria = new Dictionary<string, float>();
        TotalDespesas = ObterTotalDespesas();
    }





    private static decimal ObterSaldoDoBancoDeDados() => 0m;
    private static decimal ObterReceita() => 0m;
    private static decimal ObterTotalDespesas() => 0m;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

