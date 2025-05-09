using Microsoft .Maui.Controls; 
namespace FinancaPlus.Views;


public partial class ConfiguracaoPage : ContentPage
{
    private Entry txt_email;
    public ConfiguracaoPage()
	{
		InitializeComponent();
        txt_email = new Entry();

    }

    
 
private async void perfil_Clicked(object sender, EventArgs e)
    {   // Aqui você pode navegar para a página de perfil
        await Navigation.PushAsync(new Perfil(txt_email.Text));

    }

    private void BTN_trocarSenha_Clicked(object sender, EventArgs e)
    {

    }

    private async void BTN_salvarSaldo_Clicked(object sender, EventArgs e)
    {
        try
        {
            decimal novoSaldo;

            if (decimal.TryParse(EntrySaldo.Text, out novoSaldo))
            {
                Preferences.Set("SaldoInicial", novoSaldo.ToString());
                LBL_saldo.Text = $"Saldo: R$ {novoSaldo:N2}";

                DisplayAlert("Sucesso", "Saldo salvo com sucesso!", "OK");
                // Voltar para a tela anterior (TelaPrincipal)
                await Navigation.PushAsync(new TelaPrincipal(txt_email.Text));
            }
            else
            {
                DisplayAlert("Erro", "Insira um valor válido!", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao salvar saldo: {ex.Message}", "OK");
        }
    }



    private void BTN_logout_Clicked(object sender, EventArgs e)
    {

    }
}