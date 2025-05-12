using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Threading.Tasks;

namespace FinancaPlus.Views;

public partial class Perfil : ContentPage
{
	private SQLiteDatabaseHelpers db;
	private Usuario usuarioAtual;

	private Entry txt_nome;
    private Entry txt_email;
    private Entry txt_senha;
    public Perfil(string email)
	{
		InitializeComponent();
		db = new SQLiteDatabaseHelpers();
		usuarioAtual = db.GetUsuario(email);

        txt_nome = new Entry(); 
        txt_email = new Entry();
        txt_senha = new Entry { IsPassword = true};

        if (usuarioAtual != null)
		{
			txt_nome.Text = usuarioAtual.Nome;
            txt_email.Text = usuarioAtual.Email;
		}
	}

    private async void atualizarPerfil_Clicked(object sender, EventArgs e)
    {
		try 
		{
			usuarioAtual.Nome = txt_nome.Text;
			usuarioAtual.SenhaHash = txt_senha.Text;

			db.UpdateUsuario(usuarioAtual);
			await DisplayAlert("Sucesso", "Perfil atualizado com sucesso", "OK");

		}
		catch (Exception ex) 
		{
			await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Fechar");

		}


    }

    private void BTN_editarPerfil_Clicked(object sender, EventArgs e)
    {

    }

    private void BTN_sair_Clicked(object sender, EventArgs e)
    {

    }

    private void BTN_alterarSenha_Clicked(object sender, EventArgs e)
    {

    }

    private void BTN_sair_Clicked_1(object sender, EventArgs e)
    {

    }

    private void BTN_alterarImagem_Clicked(object sender, EventArgs e)
    {

    }
}