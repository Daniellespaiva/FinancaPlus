using Microcharts;
using Microcharts.Maui;
using SkiaSharp;

namespace FinancaPlus.Views;

public partial class TelaPrincipal : ContentPage
{
    private Entry txt_email;
  
	public TelaPrincipal()
	{
		InitializeComponent();
         txt_email = new Entry();
         
         CarregarGrafico();
       
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

    private async void perfil_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Perfil(txt_email.Text));

    }
}