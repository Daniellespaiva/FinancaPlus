using FinancaPlus.Helpers;

namespace FinancaPlus.Views;

public partial class RecuperarSenha : ContentPage
{
    public RecuperarSenha()// Construtor sem parâmetros
    {
        InitializeComponent();


    }
    public RecuperarSenha(string email) // Construtor que recebe um email
    {
        InitializeComponent();
        EntryEmail.Text = email; // Preenche o campo automaticamente
    }





    private async void BTN_cancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BTN_alterarSenha_Clicked(object sender, EventArgs e)
    {
        string email = EntryEmail.Text;
        string novaSenha = EntryNovaSenha.Text;

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            await DisplayAlert("Erro", "Digite um e-mail válido.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(novaSenha) || novaSenha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        var dbHelper = new SQLiteDatabaseHelpers();
        var usuario = dbHelper.GetUsuario(email);

        if (usuario == null)
        {
            await DisplayAlert("Erro", "E-mail não encontrado!", "OK");
            return;
        }

        // Atualizar a senha no banco de dados
        dbHelper.UpdateSenha(email, novaSenha);
        await DisplayAlert("Sucesso", "Sua senha foi alterada!", "OK");

        await Navigation.PopToRootAsync(); // Retornar à tela de login
    }
}

