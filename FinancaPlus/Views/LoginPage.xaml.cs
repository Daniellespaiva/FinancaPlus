using FinancaPlus.Helpers;

namespace FinancaPlus.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void BTN_Entrar_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Verificação antes de consultar o banco
            if (string.IsNullOrWhiteSpace(txt_email.Text) || string.IsNullOrWhiteSpace(txt_senha.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "Fechar");
                return;
            }

            var db = new SQLiteDatabaseHelpers();
            var usuario = db.GetUsuario(txt_email.Text);  // Busca usuário pelo e-mail

            if (usuario == null)
            {
                await DisplayAlert("Erro", "E-mail não cadastrado ou senha incorreta.", "Fechar");
                return;
            }
            // Agora verifica a senha corretamente
            if (!usuario.VerificarSenha(txt_senha.Text))
            {
                await DisplayAlert("Erro", "Senha incorreta.", "Fechar");
                return;
            }

            await DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");

            // Navega para a tela principal passando o email correto do usuário
            await Navigation.PushAsync(new TelaPrincipal(usuario.Email));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar login: {ex.Message}");
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Fechar");
        }

    }
    private async void BTN_Registrar_Clicked(object sender, EventArgs e)
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

    private async void BTN_RecuperarSenha_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_email.Text))
        {
            await DisplayAlert("Erro", "Digite seu e-mail para recuperar a senha.", "Fechar");
            return;
        }

        await Navigation.PushAsync(new RecuperarSenha(txt_email.Text));
    }


}


