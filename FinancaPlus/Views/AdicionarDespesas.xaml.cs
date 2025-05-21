using FinancaPlus.Helpers;
using FinancaPlus.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FinancaPlus.Views;

public partial class AdicionarDespesas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    public ObservableCollection<Gasto> ListaDespesas { get; set; } = new ObservableCollection<Gasto>();

    public AdicionarDespesas()
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        ListaDespesas = new ObservableCollection<Gasto>(_dbHelpers.GetDespesas());

        DespesasListView.ItemsSource = ListaDespesas;
    }

    private async void BTN_AdicionarDespesa_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EntryNomeDespesa.Text) || string.IsNullOrWhiteSpace(EntryValorDespesa.Text) || PickerCategoria.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Preencha todos os campos!", "Fechar");
                return;
            }

            if (!decimal.TryParse(EntryValorDespesa.Text, out decimal valorDespesa))
            {
                await DisplayAlert("Erro", "Insira um valor válido!", "Fechar");
                return;
            }

            var novaDespesa = new Gasto
            {
                Categoria = PickerCategoria.SelectedItem?.ToString(),
                GastoPorGateria = valorDespesa, // Substituindo "Valor" por "GastoPorGateria"
                Data = PickerDataDespesa.Date
            };


            // Adicionando o nome da despesa como uma propriedade adicional
            novaDespesa.GetType().GetProperty("Nome")?.SetValue(novaDespesa, EntryNomeDespesa.Text);

            _dbHelpers.AddDespesa(novaDespesa);
            ListaDespesas.Add(novaDespesa);

            // **Envia mensagem para atualizar saldo na TelaPrincipal**
            MessagingCenter.Send(this, "AtualizarDespesas", valorDespesa);

            await DisplayAlert("Sucesso", $"Despesa adicionada!", "OK");

            EntryNomeDespesa.Text = string.Empty;
            EntryValorDespesa.Text = string.Empty;
            PickerCategoria.SelectedItem = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao adicionar a despesa: {ex.Message}", "OK");
        }
    }
}