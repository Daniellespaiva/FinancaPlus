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

    private void BTN_salvarSaldo_Clicked(object sender, EventArgs e)
    {

    }

    private void BTN_logout_Clicked(object sender, EventArgs e)
    {

    }
}