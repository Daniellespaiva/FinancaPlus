using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FinancaPlus.Views;

public partial class DefinirReceitas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();


    public DefinirReceitas()
    {
        try
        {
            _dbHelpers = new SQLiteDatabaseHelpers(); // Inicialização do campo _dbHelpers
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas()); // Inicialização da propriedade ListaReceitas


            InitializeComponent();


            
            EntryNomeReceita = this.FindByName<Entry>("EntryNomeReceita");
            EntryValorReceita = this.FindByName<Entry>("EntryValorReceita");
            PickerCategoria = this.FindByName<Picker>("PickerCategoria");

            if (ReceitasListView != null)
            {
                ReceitasListView.ItemsSource = ListaReceitas;
            }
            else
            {
                throw new Exception("Erro ao encontrar ListView 'ReceitasListView'. Verifique o nome no XAML.");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro na inicialização: {ex.Message}", "OK");
        }
    }

    private async void BTN_AdicionarReceita_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (EntryNomeReceita == null || EntryValorReceita == null || PickerCategoria == null)
            {
                await DisplayAlert("Erro", "Os controles da interface não foram encontrados. Verifique seu XAML.", "Fechar");
                return;
            }

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

            await Navigation.PushAsync(new TelaPrincipal("example@example.com"));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao adicionar a receita: {ex.Message}", "OK");
        }
    }
}

