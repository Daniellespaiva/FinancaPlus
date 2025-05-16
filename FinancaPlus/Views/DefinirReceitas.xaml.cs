using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FinancaPlus.Views;

public partial class DefinirReceitas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    public ObservableCollection<Receita> ListaReceitas { get; set; }
    // Adicione as propriedades para os controles da interface
    private Entry EntryNomeReceita { get; set; }
    private Entry EntryValorReceita { get; set; }
    private Picker PickerCategoria { get; set; }
    private ListView ReceitasListView { get; set; } // Adicionado para corrigir o erro


    public DefinirReceitas()
    {
        InitializeComponent(); // Certifique-se de que este método seja chamado para inicializar os componentes da interface.
        _dbHelpers = new SQLiteDatabaseHelpers();
        ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());
        ReceitasListView = this.FindByName<ListView>("ReceitasListView"); // Inicialização corrigida
        ReceitasListView.ItemsSource = ListaReceitas;


        // Inicialize os controles da interface
        EntryNomeReceita = this.FindByName<Entry>("EntryNomeReceita");
        EntryValorReceita = this.FindByName<Entry>("EntryValorReceita");
        PickerCategoria = this.FindByName<Picker>("PickerCategoria");
    }

    

    private async void BTN_AdicionarReceita_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNomeReceita.Text) || string.IsNullOrWhiteSpace(EntryValorReceita.Text) || PickerCategoria.SelectedItem == null)
        {
            await DisplayAlert("Erro", "Preencha todos os campos!", "Fechar");
            return;
        }

        if (!decimal.TryParse(EntryValorReceita.Text, out decimal valorReceita))
        {
            await DisplayAlert("Erro", "Insira um valor válido!", "Fechar");
            return;
        }
        var selectedCategoria = PickerCategoria.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedCategoria))
        {
            await DisplayAlert("Erro", "Selecione uma categoria válida!", "Fechar");
            return;
        }


        var novaReceita = new Receita
        {
            Nome = EntryNomeReceita.Text,
            Valor = valorReceita,
            Categoria = selectedCategoria
        };

        _dbHelpers.AddReceita(novaReceita);
        ListaReceitas.Add(novaReceita);

        await DisplayAlert("Sucesso", $"Receita '{novaReceita.Nome}' adicionada com sucesso no valor de R$ {novaReceita.Valor:N2}!", "OK");

        EntryNomeReceita.Text = string.Empty;
        EntryValorReceita.Text = string.Empty;
        PickerCategoria.SelectedItem = null;



        // **Redireciona para Tela Principal**
        await Navigation.PushAsync(new TelaPrincipal("example@example.com")); // Ajuste conforme seu banco de dados    }
    }
}

