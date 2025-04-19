namespace FinancaPlus.Views;

public partial class TelaPrincipal : ContentPage
{
    private Entry txt_email;
	public TelaPrincipal()
	{
		InitializeComponent();
        txt_email = new Entry();
	}

    private async void BTN_receitas_Clicked(object sender, EventArgs e)
    {
        try 
        {
            await Navigation.PushAsync(new DefinirReceitas());

        }
        catch 
        {

        }
    }

    private async void BTN_despesas_Clicked(object sender, EventArgs e)
    {
        try 
        {
            await Navigation.PushAsync(new AdicionarDespesas());
        }
        catch 
        {

        }
    }

    private async void BTN_definirMetas_Clicked(object sender, EventArgs e)
    {
        try 
        {
            await Navigation.PushAsync(new DefinirMetas());
        }
        catch 
        {

        }
    }

    private async void BTN_relatorios_Clicked(object sender, EventArgs e)
    {
        try 
        {
            await Navigation.PushAsync(new GerarRelatorios());

        }
        catch 
        {

        }
    }

    private async void perfil_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Perfil(txt_email.Text));

    }
}