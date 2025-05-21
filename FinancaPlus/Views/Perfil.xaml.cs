using FinancaPlus.Helpers;
using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;

namespace FinancaPlus.Views;

public partial class Perfil : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private readonly Usuario _usuarioAtual;

    // Declara��o dos campos de entrada como vari�veis de inst�ncia
    private Entry _entryNomeUsuario;
    private Entry _entryEmail;
    private Entry _entryTelefone; // Renomeado para evitar ambiguidade

    public Perfil(string email)
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _usuarioAtual = _dbHelpers.GetUsuario(email) ?? throw new InvalidOperationException("Usu�rio n�o encontrado.");

        if (_usuarioAtual != null)
        {
            BindingContext = _usuarioAtual;

           // Refer�ncia aos campos de entrada no XAML
            _entryNomeUsuario = this.FindByName<Entry>("EntryNomeUsuario");
            _entryEmail = this.FindByName<Entry>("EntryEmail");
            _entryTelefone = this.FindByName<Entry>("EntryTelefone");

            // Preenchimento dos campos com os dados do usu�rio
            _entryNomeUsuario.Text = _usuarioAtual.Nome;
            _entryEmail.Text = _usuarioAtual.Email;
            _entryTelefone.Text = _usuarioAtual.Telefone;
        }
        else
        {
            DisplayAlert("Erro", "Usu�rio n�o encontrado!", "OK");
        }
    }
    

    private async void SalvarPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_usuarioAtual == null || _entryNomeUsuario == null || _entryEmail == null || _entryTelefone == null)
            {
                await DisplayAlert("Erro", "N�o foi poss�vel salvar o perfil. Campos inv�lidos!", "OK");
                return;
            }

            _usuarioAtual.Nome = _entryNomeUsuario.Text;
            _usuarioAtual.Email = _entryEmail.Text;
            _usuarioAtual.Telefone = _entryTelefone.Text;

            _dbHelpers.UpdateUsuario(_usuarioAtual);

            await DisplayAlert("Sucesso", "Perfil atualizado com sucesso!", "OK");

            // Voltar para a tela principal com email atualizado
            await Navigation.PushAsync(new TelaPrincipal(_usuarioAtual.Email));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao salvar perfil: {ex.Message}", "OK");
        }
    }
    private async void BTN_Sair_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
