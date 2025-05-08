using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;

namespace FinancaPlus.Views;

public partial class TelaPrincipal : ContentPage
{

  
	public TelaPrincipal(string email)
	{
		InitializeComponent();
                
         CarregarGrafico();
        BindingContext = new TelaPrincipalViewModel(email);

    }

    public class TelaPrincipalViewModel 
    {
        private readonly SQLiteDatabaseHelpers _dbHelpers;
        public Usuario UsuarioLogado { get; set; }
        public TelaPrincipalViewModel(string email)
        {
            _dbHelpers = new SQLiteDatabaseHelpers();
            UsuarioLogado = _dbHelpers.GetUsuario(email);
        }
       
    }
     public void AtualizarSaldo()
    {
        decimal saldo = ObterSaldoDoBancoDeDados();
        LBL_saldo.Text = $"R$ {saldo:N2}";
    }
    private decimal ObterSaldoDoBancoDeDados()
    {
        return 0; // Retorne o valor real do saldo
    }
    private void CarregarGrafico()
    {
        
        var entrada = new[]
        {
            new ChartEntry(30) { Label = "Moradia", ValueLabel = "30%", Color = SKColor.Parse("#FF5733") },
            new ChartEntry(25) { Label = "Alimento", ValueLabel = "25%", Color = SKColor.Parse("#33FF57") },
            new ChartEntry(20) { Label = "Transporte", ValueLabel = "20%", Color = SKColor.Parse("#3357FF") },
            new ChartEntry(25) { Label = "Outros", ValueLabel = "25%", Color = SKColor.Parse("#FF33A5") }
        };

       
    
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