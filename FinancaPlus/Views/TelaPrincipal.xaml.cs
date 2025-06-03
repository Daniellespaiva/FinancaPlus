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
        _viewModel.AtualizarGastosPorCategoria();

        IniciarAtualizacaoAutomatica(); // Inicia a atualização automática dos valores



        WeakReferenceMessenger.Default.Register<AtualizarFinanceiroMessage>(this, (recipient, message) =>
        {
            if (recipient is TelaPrincipal telaPrincipal)
            {
                telaPrincipal._viewModel.SaldoDisponivel = message.NovaReceita - message.NovaDespesa;
                telaPrincipal._viewModel.ReceitaAtual = message.NovaReceita;
                telaPrincipal._viewModel.TotalDespesas = message.NovaDespesa;
                telaPrincipal._viewModel.AtualizarSaldo(); // Recarrega os dados após a atualização
                telaPrincipal._viewModel.AtualizarGastosPorCategoria(); // Recalcula e exibe os percentuais das categorias em tempo real
                telaPrincipal._viewModel.AtualizarTransacoesRecentes(); // Atualiza transações em tempo real
            }



        });

        // Certifique-se de que os nomes das Labels correspondem aos IDs definidos no arquivo XAML
        LBL_NomeUsuario = this.FindByName<Label>("LBL_NomeUsuario");
        LBL_Saldo = this.FindByName<Label>("LBL_SaldoDisponivel"); // Adiciona a referência correta para LBL_Saldo
        LBL_Despesas = this.FindByName<Label>("LBL_TotalDespesas"); // Adiciona a referência correta para LBL_Despesas
        LBL_Receita = this.FindByName<Label>("LBL_Receita"); // Adiciona a referência correta para LBL_Receita
      

        CarregarNomeUsuario();
      
    }

    private async void IniciarAtualizacaoAutomatica()
    {
        while (true)
        {
            _viewModel.AtualizarSaldo(); // Atualiza os valores do banco
            _viewModel.AtualizarTransacoesRecentes(); // Atualiza trans
            await Task.Delay(1800000); // Aguarda 30 minutos antes da próxima atualização
        }
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

        AtualizarGastosPorCategoria(); // Chamar atualização de gastos por categoria sempre que o saldo mudar
        AtualizarTransacoesRecentes(); // Atualiza a lista de transações


        OnPropertyChanged(nameof(SaldoDisponivel));
        OnPropertyChanged(nameof(ReceitaAtual));
        OnPropertyChanged(nameof(TotalDespesas));


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

    public void AtualizarTransacoesRecentes()
    {
        var transacoes = _dbHelpers.ObterTransacoesRecentes(); // Método que retorna a lista do banco
        TransacoesRecentes.Clear(); // Limpa a lista antiga

        foreach (var transacao in transacoes)
        {
            TransacoesRecentes.Add(new Transacao
            {
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                CorValor = transacao.Valor < 0 ? "Red" : "Green" // Define cor conforme positivo/negativo
            });
        }

        OnPropertyChanged(nameof(TransacoesRecentes)); // Notifica a interface para atualizar
    }
    public void AtualizarGastosPorCategoria()
    {
        var totalDespesas = _dbHelpers.ObterTotalDespesas();
        var despesasPorCategoria = _dbHelpers.ObterDespesasPorCategoria(); // Método que retorna um dicionário {Categoria: Valor}

        GastosPorCategoria.Clear();

        foreach (var categoria in despesasPorCategoria)
        {
            decimal percentual = totalDespesas > 0 ? (categoria.Value / totalDespesas) * 100 : 0;
            GastosPorCategoria.Add(new GastoCategoria
            {
                Nome = categoria.Key,
                Percentual = (float)(percentual / 100), // Corrigido para conversão explícita de decimal para float
                CategoriaCor = DefinirCorPorCategoria(categoria.Key)
            });
        }

        OnPropertyChanged(nameof(GastosPorCategoria));
    }

    private string DefinirCorPorCategoria(string categoria)
    {
        return categoria switch
        {
            "Moradia" => "Blue",
            "Supermercado" => "Green",
            "Saúde" => "Yellow",
            "Educação" => "Green",
            "Transporte" => "Red",
            "Outros" => "Purple",
            _ => "Gray" // Cor padrão para categorias desconhecidas
        };
    }
}

