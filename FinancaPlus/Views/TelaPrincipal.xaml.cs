using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
using System.ComponentModel;
using System.Diagnostics;

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
        _viewModel.SaldoInicial = decimal.TryParse(Preferences.Get("SaldoInicial", "0"), out decimal saldo) ? saldo : 0m;

        BindingContext = _viewModel;

        // Certifique-se de que os nomes das Labels correspondem aos IDs definidos no arquivo XAML
        LBL_NomeUsuario = this.FindByName<Label>("LBL_NomeUsuario");
        LBL_Saldo = this.FindByName<Label>("LBL_Saldo"); // Adiciona a refer�ncia correta para LBL_Saldo
        LBL_Despesas = this.FindByName<Label>("LBL_Despesas"); // Adiciona a refer�ncia correta para LBL_Despesas

        CarregarNomeUsuario();
        // **Atualiza a Label automaticamente quando o saldo for modificado**
        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(_viewModel.SaldoInicial))
            {
                LBL_Saldo.Text = $"Saldo: R$ {_viewModel.SaldoInicial:N2}";
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
            Debug.WriteLine("Erro: Nome do usu�rio n�o p�de ser carregado.");
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
        await Navigation.PushAsync(new GerarRelatorio()); // Abre tela de relat�rios
    }

    private async void IrParaMetas_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DefinirMetas()); // Abre metas
    }

    private async void IrParaLogout_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync(); // Faz logout e retorna � tela inicial
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

    private async void BTN_despesas_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new AdicionarDespesas());
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
}
 
public partial class TelaPrincipalViewModel : INotifyPropertyChanged
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private decimal _saldoInicial = 0m;
    private decimal _totalDespesas = 0m;

    public event PropertyChangedEventHandler? PropertyChanged;

    // Propriedade n�o anul�vel inicializada no construtor
    public Usuario UsuarioLogado { get; set; } = new Usuario();
    public Gasto Gasto { get; set; } = new Gasto(); // Inicializa os gastos
    public Dictionary<string, float> GastosPorCategoria { get; set; }

    public decimal SaldoInicial
    {
        get => _saldoInicial;
        set
        {
            _saldoInicial = value;
            OnPropertyChanged(nameof(SaldoInicial)); // Notifica a UI
            OnPropertyChanged(nameof(Saldo));
        }
    }

    public decimal TotalDespesas
    {
        get => _totalDespesas;
        set
        {
            _totalDespesas = value;
            OnPropertyChanged(nameof(TotalDespesas)); // Notifica a UI
            OnPropertyChanged(nameof(Saldo));
        }
    }

    public decimal Saldo => SaldoInicial - TotalDespesas;
   
    public TelaPrincipalViewModel(string email)
    {
        _dbHelpers = new SQLiteDatabaseHelpers();
        UsuarioLogado = _dbHelpers.GetUsuario(email) ?? new Usuario
        {
            Nome = string.Empty,
            Email = string.Empty
        };
        SaldoInicial = ObterSaldoDoBancoDeDados();
        TotalDespesas = ObterTotalDespesas();
        GastosPorCategoria = new Dictionary<string, float>();

        // **Inscri��o para atualizar saldo via MessagingCenter**
        MessagingCenter.Subscribe<DefinirReceitas, decimal>(this, "AtualizarSaldo", (sender, valorReceita) =>
        {
            SaldoInicial += valorReceita;
            Preferences.Set("SaldoInicial", SaldoInicial.ToString());
            OnPropertyChanged(nameof(SaldoInicial)); // Atualiza a UI
        });

        MessagingCenter.Subscribe<AdicionarDespesas, decimal>(this, "AtualizarDespesas", (sender, valorDespesa) =>
        {
            TotalDespesas += valorDespesa;
            Preferences.Set("TotalDespesas", TotalDespesas.ToString());
            OnPropertyChanged(nameof(TotalDespesas)); // Atualiza a UI
        });        

    }

    private static decimal ObterSaldoDoBancoDeDados()
    {
        return 0m; // Pegue saldo real do banco
    }

    private static decimal ObterTotalDespesas()
    {
        return 0m; // Pegue despesas reais do banco
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
