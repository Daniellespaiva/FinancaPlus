using Microsoft .Maui.Controls; 
using FinancaPlus.Helpers;
using System;

namespace FinancaPlus.Views;

public partial class ConfiguracaoPage : ContentPage
{
    private Entry txt_email; 

    public ConfiguracaoPage()
    {
        InitializeComponent();

        // Inicializa os componentes
        txt_email = this.FindByName<Entry>("txt_email");
       
    }  
    private async void BTN_trocarSenha_Clicked(object sender, EventArgs e)
    {
        if (txt_email == null || string.IsNullOrWhiteSpace(txt_email.Text))
        {
            await DisplayAlert("Erro", "Insira um email válido para trocar a senha.", "Fechar");
            return;
        }

        await Navigation.PushAsync(new RecuperarSenha(txt_email.Text));
    }

   
    private async void BTN_logout_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
        private void BTN_voltar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}