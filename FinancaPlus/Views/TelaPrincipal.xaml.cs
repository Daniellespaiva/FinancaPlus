using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
using System.Collections.ObjectModel;
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
        
        BindingContext = _viewModel; // Define o contexto de dados para a ViewModel
        _viewModel.UsuarioLogado = _dbHelpers.GetUsuario(email) ?? new Usuario();

        _viewModel.AtualizarSaldo(); // Atualiza valores assim que a tela abrir



        WeakReferenceMessenger.Default.Register<AtualizarFinanceiroMessage>(this, (recipient, message) =>
        {
            if (recipient is TelaPrincipal telaPrincipal)
            {
                telaPrincipal._viewModel.SaldoDisponivel = message.NovaReceita - message.NovaDespesa;
                telaPrincipal._viewModel.ReceitaAtual = message.NovaReceita;
                telaPrincipal._viewModel.TotalDespesas = message.NovaDespesa;
            }
        });

        // Certifique-se de que os nomes das Labels correspondem aos IDs definidos no arquivo XAML
        LBL_NomeUsuario = this.FindByName<Label>("LBL_NomeUsuario");
        LBL_Saldo = this.FindByName<Label>("LBL_SaldoDisponivel"); // Adiciona a referência correta para LBL_Saldo
        LBL_Despesas = this.FindByName<Label>("LBL_TotalDespesas"); // Adiciona a referência correta para LBL_Despesas
        LBL_Receita = this.FindByName<Label>("LBL_Receita"); // Adiciona a referência correta para LBL_Receita
      

        CarregarNomeUsuario();
      
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

    private async void IrParaMetas_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DefinirMetas());
    }

    private async void BTN_Despesas_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TelaCategorizacao());
    }
}

public partial class TelaPrincipalViewModel : INotifyPropertyChanged
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private decimal _receitaAtual;
    private decimal _totalDespesas;
    private string _categoriaSelecionada;
    private decimal _saldoDisponivel;


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public decimal SaldoDisponivel
    {
        get => _saldoDisponivel;
        set
        {
            _saldoDisponivel = value;
            OnPropertyChanged(nameof(SaldoDisponivel));
        }
    }

    public decimal ReceitaAtual
    {
        get => _receitaAtual;
        set
        {
            _receitaAtual = value;
            OnPropertyChanged(nameof(ReceitaAtual));
        }
    }

    public decimal TotalDespesas
    {
        get => _totalDespesas;
        set
        {
            _totalDespesas = value;
            OnPropertyChanged(nameof(TotalDespesas));
        }
    }



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

    public string CategoriaSelecionada
    {
        get => _categoriaSelecionada;
        set
        {
            _categoriaSelecionada = value;
            OnPropertyChanged(nameof(CategoriaSelecionada));
        }
    }



    public void AtualizarSaldo()
    {
        ReceitaAtual = _dbHelpers.ObterTotalReceita();
        TotalDespesas = _dbHelpers.ObterTotalDespesas();
        SaldoDisponivel = ReceitaAtual - TotalDespesas;

    }



    // Propriedade não anulável inicializada no construtor
    public Usuario UsuarioLogado { get; set; } = new Usuario();
    public Despesa Gasto { get; set; } = new Despesa(); // Inicializa os gastos

    public ObservableCollection<GastoCategoria> GastosPorCategoria { get; set; }
    public ObservableCollection<CategoriaDespesa> CategoriasDespesas { get; }
    public ObservableCollection<Transacao> TransacoesRecentes { get; set; }



    public TelaPrincipalViewModel(string email)
    {

        _dbHelpers = new SQLiteDatabaseHelpers();
        UsuarioLogado = _dbHelpers.GetUsuario(email) ?? new Usuario
        {
            Nome = string.Empty,
            Email = string.Empty
        };
       

        GastosPorCategoria = new ObservableCollection<GastoCategoria>
    {
        new GastoCategoria { Nome = "Moradia", Percentual = 0f, CategoriaCor = "Blue" },
        new GastoCategoria { Nome = "Supermercado", Percentual = 0f, CategoriaCor = "Green" },
        new GastoCategoria { Nome = "Saúde", Percentual = 0f, CategoriaCor = "Yellow" },
        new GastoCategoria { Nome = "Educação", Percentual = 0f, CategoriaCor = "Green" },
        new GastoCategoria { Nome = "Transporte", Percentual = 0f, CategoriaCor = "Red" },
        new GastoCategoria { Nome = "Outros", Percentual = 0.20f, CategoriaCor = "Purple" }
    };

       


        TransacoesRecentes = new ObservableCollection<Transacao>
        {
            new Transacao { Descricao = "Supermercado", Valor = -150.00m, CorValor = "Red" },
            new Transacao { Descricao = "Salário", Valor = 3000.00m, CorValor = "Green" }
        };

    }
  
    


}

