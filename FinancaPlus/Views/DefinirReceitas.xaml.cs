using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace FinancaPlus.Views;

public partial class DefinirReceitas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();

    // Comando para excluir uma receita
    public ICommand DeleteCommand { get; private set; } = null!;

    public DefinirReceitas()
    {
        _dbHelpers = new SQLiteDatabaseHelpers(); // Inicialização do campo _dbHelpers
        try
        {
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());
            DeleteCommand = new Command<Receita>(ExcluirReceita);

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

            var categoriaSelecionada = PickerCategoria.SelectedItem?.ToString();
            if (categoriaSelecionada == null)
            {
                await DisplayAlert("Erro", "Selecione uma categoria válida!", "Fechar");
                return;
            }

            var novaReceita = new Receita
            {
                Nome = EntryNomeReceita.Text,
                Valor = valorReceita,
                Categoria = categoriaSelecionada
            };

            _dbHelpers.AddReceita(novaReceita);
            ListaReceitas.Add(novaReceita);

            // **Envia mensagem para atualizar o saldo na TelaPrincipal**
            WeakReferenceMessenger.Default.Send(new AtualizarSaldoMessage(valorReceita));
            await DisplayAlert("Sucesso", $"Receita '{novaReceita.Nome}' adicionada!", "OK");

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

    // Crie uma classe para encapsular a mensagem
    public class AtualizarSaldoMessage
    {
        public decimal Valor { get; }

        public AtualizarSaldoMessage(decimal valor)
        {
            Valor = valor;
        }
    }


    // **Método para excluir receita**
    private void ExcluirReceita(Receita receita)
    {
        if (receita != null)
        {
            Debug.WriteLine($"Tentando excluir receita: {receita.Nome}");

            _dbHelpers.RemoverReceita(receita);
            ListaReceitas.Remove(receita);

            MessagingCenter.Send(this, "AtualizarSaldo", -receita.Valor);
        }
    }
}



