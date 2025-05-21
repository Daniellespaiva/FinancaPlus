using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microsoft.Maui.Controls;  

namespace FinancaPlus.Views;

public partial class RecuperarSenha : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelper;
    public RecuperarSenha()// Construtor sem parâmetros
    {
        InitializeComponent();
        _dbHelper = new SQLiteDatabaseHelpers();

        // Inicializando campos
        EntryEmail = this.FindByName<Entry>("EntryEmail");
        EntryNovaSenha = this.FindByName<Entry>("EntryNovaSenha");
    }

    public RecuperarSenha(string email) : this() // Construtor que recebe email
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            EntryEmail.Text = email; // Preenche o campo automaticamente
        }
    }

    private async void BTN_Cancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BTN_AlterarSenha_Clicked(object sender, EventArgs e)
    {
        if (EntryEmail == null || EntryNovaSenha == null)
        {
            await DisplayAlert("Erro", "Campos inválidos! Tente novamente.", "OK");
            return;
        }

        string email = EntryEmail.Text;
        string novaSenha = EntryNovaSenha.Text;

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            await DisplayAlert("Erro", "Digite um e-mail válido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        var usuario = _dbHelper.GetUsuario(email);

        if (usuario == null)
        {
            await DisplayAlert("Erro", "E-mail não encontrado!", "OK");
            return;
        }

        // Criptografa a senha antes de atualizar no banco
        usuario.DefinirSenha(novaSenha);
        _dbHelper.UpdateUsuario(usuario);

        await DisplayAlert("Sucesso", "Sua senha foi alterada!", "OK");

        await Navigation.PopToRootAsync(); // Retornar à tela de login
    }
    private async void BTN_cancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BTN_alterarSenha_Clicked(object sender, EventArgs e)
    {
        string email = EntryEmail.Text;
        string novaSenha = EntryNovaSenha.Text;

        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
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
        dbHelper.AtualizarSenha(email, novaSenha);
        await DisplayAlert("Sucesso", "Sua senha foi alterada!", "OK");

        await Navigation.PopToRootAsync(); // Retornar à tela de login
    }
}

