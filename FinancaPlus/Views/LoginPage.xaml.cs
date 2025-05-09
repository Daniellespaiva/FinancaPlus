using FinancaPlus.Helpers;

namespace FinancaPlus.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void btn_entar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var db = new SQLiteDatabaseHelpers();
            var usuario = db.GetUsuario(txt_email.Text, txt_senha.Text);
            if (usuario == null)
            {
                await DisplayAlert("Erro", "E-mail n�o cadastrado ou senha incorreta.", "Fechar");
                return;
            }    
                await DisplayAlert("Sucesso", "Login realizado com sucesso!", "Fechar");
                await Navigation.PushAsync(new TelaPrincipal(txt_email.Text));
                 

            if (txt_email.Text == null || txt_senha.Text == null)
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "Fechar");
                return;
            }
                     
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar login: {ex.Message}");
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Fechar");
        }
    }

    
    private async void btn_registrar_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new CadastroLogin());
        }
        catch (Exception ex)
        {
           await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Fechar");
        }

    }

    private async void BTN_recuperarSenha_Clicked(object sender, EventArgs e)
    {
        string emailUsuario = txt_email.Text;
        await Navigation.PushAsync(new RecuperarSenha(emailUsuario));
    }

}


