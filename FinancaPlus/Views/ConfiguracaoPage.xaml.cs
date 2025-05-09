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
    {   // Aqui voc� pode navegar para a p�gina de perfil
        await Navigation.PushAsync(new Perfil(txt_email.Text));

    }

    private async void BTN_trocarSenha_Clicked(object sender, EventArgs e)
    {
        string emailUsuario = txt_email.Text;
        await Navigation.PushAsync(new RecuperarSenha(emailUsuario));

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
                DisplayAlert("Erro", "Insira um valor v�lido!", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao salvar saldo: {ex.Message}", "OK");
        }
    }



    private async void BTN_logout_Clicked(object sender, EventArgs e)
    {

        await Navigation.PopToRootAsync();

    }
}