using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using System.ComponentModel;

namespace FinancaPlus.Views;

public partial class TelaPrincipal : ContentPage
{
    private readonly TelaPrincipalViewModel _viewModel;

    public TelaPrincipal(string email)
    {
        InitializeComponent();
        _viewModel = new TelaPrincipalViewModel(email);

        // Se o saldo inicial não estiver salvo, começa em 0
        _viewModel.SaldoInicial = decimal.Parse(Preferences.Get("SaldoInicial", 0m.ToString()));

        BindingContext = _viewModel;

        CarregarGrafico();
    }

    private void CarregarGrafico()
    {
        var entrada = new[]
        {
            new ChartEntry(30) { Label = "Moradia", ValueLabel = "0%", Color = SKColor.Parse("#FF5733") },
            new ChartEntry(25) { Label = "Alimento", ValueLabel = "0%", Color = SKColor.Parse("#33FF57") },
            new ChartEntry(20) { Label = "Transporte", ValueLabel = "0%", Color = SKColor.Parse("#3357FF") },
            new ChartEntry(25) { Label = "Outros", ValueLabel = "%", Color = SKColor.Parse("#FF33A5") }
        };
    }


    public class TelaPrincipalViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteDatabaseHelpers _dbHelpers;
        private decimal _saldoInicial = 0m;
        private decimal _totalDespesas = 0m;

        public event PropertyChangedEventHandler PropertyChanged;

        public Usuario UsuarioLogado { get; set; }
        public AdicionarDespesas Gastos { get; set; } // Instância do GastoViewModel

        public decimal SaldoInicial
        {
            get => _saldoInicial;
            set
            {
                _saldoInicial = value;
                OnPropertyChanged(nameof(SaldoInicial));
                OnPropertyChanged(nameof(Saldo));
            }
        }

        public decimal TotalDespesas
        {
            get => _totalDespesas;
            set
            {
                _totalDespesas = value;
                OnPropertyChanged(nameof(TotalDespesas));
                OnPropertyChanged(nameof(Saldo));
            }
        }

        public decimal Saldo => SaldoInicial - TotalDespesas;

        public TelaPrincipalViewModel(string email)
        {
            _dbHelpers = new SQLiteDatabaseHelpers();
            UsuarioLogado = _dbHelpers.GetUsuario(email);
            SaldoInicial = ObterSaldoDoBancoDeDados();
            TotalDespesas = ObterTotalDespesas();

            Gastos = new AdicionarDespesas(); // Inicializa os gastos
        }

        private decimal ObterSaldoDoBancoDeDados()
        {
            return 1500.75m; // Pegue saldo real do banco
        }

        private decimal ObterTotalDespesas()
        {
            return 500.00m; // Pegue despesas reais do banco
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
            await Navigation.PushAsync(new GerarRelatorios());

        }
        catch 
        {

        }
    }

    
    private void BTN_configuracoes_Clicked(object sender, EventArgs e)
    {
        // Redireciona para a página de configurações
        Navigation.PushAsync(new ConfiguracaoPage());
    }
}