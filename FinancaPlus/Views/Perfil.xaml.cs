using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Threading.Tasks;

namespace FinancaPlus.Views;

public partial class Perfil : ContentPage
{
	private SQLiteDatabaseHelpers db;
	private Usuario usuarioAtual;
	public Perfil(string email)
	{
		InitializeComponent();
		db = new SQLiteDatabaseHelpers();
		usuarioAtual = db.GetUsuario(email);

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
			usuarioAtual.Senha = txt_senha.Text;

			db.UpdateUsuario(usuarioAtual);
			await DisplayAlert("Sucesso", "Perfil atualizado com sucesso", "OK");

		}
		catch (Exception ex) 
		{
			await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Fechar");

		}


    }
}