using CommunityToolkit.Mvvm.Messaging;
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
        LBL_Saldo = this.FindByName<Label>("LBL_Saldo"); // Adiciona a referência correta para LBL_Saldo
        LBL_Despesas = this.FindByName<Label>("LBL_Despesas"); // Adiciona a referência correta para LBL_Despesas

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

    private async void BTN_despesas_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new AdicionarDespesas());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao abrir a tela de despesas: {ex.Message}", "OK");
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

    // Propriedade não anulável inicializada no construtor
    public Usuario UsuarioLogado { get; set; } = new Usuario();
    public Gasto Gasto { get; set; } = new Gasto(); // Inicializa os gastos
    public Dictionary<string, float> GastosPorCategoria { get; set; }

    public decimal SaldoInicial
    {
        get => _saldoInicial;
        set
        {
            _saldoInicial = value;
            Preferences.Set("SaldoInicial", _saldoInicial.ToString());
            OnPropertyChanged(nameof(SaldoInicial));
            OnPropertyChanged(nameof(Saldo)); // Atualiza o saldo total
        }
    }

    public decimal TotalDespesas
    {
        get => _totalDespesas;
        set
        {
            _totalDespesas = value;
            Preferences.Set("TotalDespesas", _totalDespesas.ToString());
            OnPropertyChanged(nameof(TotalDespesas));
            OnPropertyChanged(nameof(Saldo)); // Atualiza o saldo total
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

        WeakReferenceMessenger.Default.Register<AtualizarSaldoMessage>(this, (recipient, msg) =>
        {
            if (recipient is TelaPrincipalViewModel viewModel)
            {
                viewModel.SaldoInicial += msg.Value; // Usa `msg.Value` corretamente
                Preferences.Set("SaldoInicial", viewModel.SaldoInicial.ToString());
                viewModel.OnPropertyChanged(nameof(viewModel.SaldoInicial)); // Atualiza a UI automaticamente
            }
        });

        WeakReferenceMessenger.Default.Register<AtualizarDespesasMessage>(this, (recipient, msg) =>
        {
            if (recipient is TelaPrincipalViewModel viewModel)
            {
                viewModel.TotalDespesas += msg.Value; // Usa `msg.Value` corretamente
                Preferences.Set("TotalDespesas", viewModel.TotalDespesas.ToString());
                viewModel.OnPropertyChanged(nameof(viewModel.TotalDespesas)); // Atualiza a UI automaticamente
            }
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
