using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
using Microsoft.Maui.Controls;

namespace FinancaPlus.Views;

public partial class CadastroLogin : ContentPage
{
	public CadastroLogin()
	{
		InitializeComponent();
	}

    private  async void BTN_cadastrar_Clicked(object sender, EventArgs e)
    {
        try 
        {
            if(string.IsNullOrWhiteSpace(txt_NomeUsuario.Text) ||
                string.IsNullOrWhiteSpace(txt_email.Text) ||
                string.IsNullOrWhiteSpace(txt_senha.Text) ||
                string.IsNullOrEmpty(Txt_confirmaSenha.Text)) 
            {
                await DisplayAlert("Ops", "Preencha todos os campos", "Fechar");
                return;
            }
            if (txt_senha.Text != Txt_confirmaSenha.Text)
            {
               await DisplayAlert("Ops", "As senhas n�o conferem", "Fechar");
                return;
            }
            // Verifica se as senhas conferem
            if (txt_senha.Text != Txt_confirmaSenha.Text)
            {
                await DisplayAlert("Erro", "As senhas n�o conferem.", "Fechar");
                return;
            }

            var usuario = new Usuario
            {
                Nome = txt_NomeUsuario.Text,
                Email = txt_email.Text,
            };
            // Criptografa a senha antes de salvar no banco
            usuario.DefinirSenha(txt_senha.Text);

            var db = new SQLiteDatabaseHelpers();
            db.AddUsuario(usuario, txt_senha.Text);


            await DisplayAlert("Sucesso", "Usu�rio cadastrado com sucesso!", "Fechar");

            // Redireciona para a tela de login
            await Navigation.PushAsync(new LoginPage());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao cadastrar usu�rio: {ex.Message}");
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Fechar");
        }
    }
    private async void BTN_voltar_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao voltar: {ex.Message}");
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Fechar");
        }
    }
}

