using CommunityToolkit.Mvvm.Messaging;
using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FinancaPlus.Views;


public partial class MinhaFinancas : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;
    private MinhaFinancasViewModel _viewModel;
    private List<Receita> ListaReceitasFiltradas = new List<Receita>();
    private List<Despesa> ListaDespesasFiltradas = new List<Despesa>();
    public List<Receita> ListaReceitas { get; set; } = new List<Receita>();
    public List<Despesa> ListaDespesas { get; set; } = new List<Despesa>();




    public MinhaFinancas()
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();
        _viewModel = new MinhaFinancasViewModel();
        BindingContext = _viewModel; // Definir Binding corretamen
        // Registrar para receber atualização do saldo

        WeakReferenceMessenger.Default.Register<AtualizarSaldoMessage>(this, (recipient, message) =>
        {
            _viewModel.SaldoDisponivel += message.Valor;
        });
    }


    public void AtualizarSaldo(decimal valor)
    {
        if (_viewModel != null)
        {
            _viewModel.SaldoDisponivel += valor; // Atualiza o saldo
            _viewModel.AtualizarSaldo(); // Atualiza a UI
        }
    }




    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string busca = e.NewTextValue.ToLower();

        ListaReceitasFiltradas = ListaReceitas.Where(r => r.Categoria.ToLower().Contains(busca)).ToList();
        ListaDespesasFiltradas = ListaDespesas.Where(d => d.Categoria.ToLower().Contains(busca)).ToList();


        OnPropertyChanged(nameof(ListaReceitasFiltradas));
        OnPropertyChanged(nameof(ListaDespesasFiltradas));

    }

    



    public class MinhaFinancasViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteDatabaseHelpers _dbHelpers;
        private decimal _saldoDisponivel;
        public decimal SaldoDisponivel
        {
            get => _saldoDisponivel;
            set
            {
                _saldoDisponivel = value;
                OnPropertyChanged(nameof(SaldoDisponivel));
            }
        }

        public ObservableCollection<Receita> ListaReceitas { get; set; } = new ObservableCollection<Receita>();
        public ObservableCollection<Despesa> ListaDespesas { get; set; } = new ObservableCollection<Despesa>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void AtualizarSaldo()
        {
            SaldoDisponivel = ListaReceitas.Sum(r => r.Valor) - ListaDespesas.Sum(d => d.Valor);
        }

        public void ApagarReceitaPorCategoria(string categoria)
        {
            var receitasParaExcluir = ListaReceitas.Where(r => r.Categoria == categoria).ToList();
            foreach (var receita in receitasParaExcluir)
            {
                ListaReceitas.Remove(receita);
            }
            AtualizarSaldo();
        }

        public void ApagarDespesaPorCategoria(string categoria)
        {
            var despesasParaExcluir = ListaDespesas.Where(d => d.Categoria == categoria).ToList();
            foreach (var despesa in despesasParaExcluir)
            {
                ListaDespesas.Remove(despesa);
            }
            AtualizarSaldo();
        }
    


    public MinhaFinancasViewModel()
        {
            _dbHelpers = new SQLiteDatabaseHelpers();
            ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());
            ListaDespesas = new ObservableCollection<Despesa>(_dbHelpers.GetDespesas());

            AtualizarSaldo(); // Inicializa o saldo com base nas receitas e despesas
        }

       
       


    private ObservableCollection<Receita> _listaReceitasFiltradas = new ObservableCollection<Receita>();
        public ObservableCollection<Receita> ListaReceitasFiltradas
        {
            get => _listaReceitasFiltradas;
            set
            {
                _listaReceitasFiltradas = value;
                OnPropertyChanged(nameof(ListaReceitasFiltradas));
            }
        }

        private ObservableCollection<Despesa> _listaDespesasFiltradas = new ObservableCollection<Despesa>();
        public ObservableCollection<Despesa> ListaDespesasFiltradas
        {
            get => _listaDespesasFiltradas;
            set
            {
                _listaDespesasFiltradas = value;
                OnPropertyChanged(nameof(ListaDespesasFiltradas));
            }
        }

        private string _categoriaSelecionada;
        public string CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                _categoriaSelecionada = value;
                OnPropertyChanged(nameof(CategoriaSelecionada));
            }
        }
    }

    private void IrParaPerfil_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaRelatorios_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaConfig_Clicked(object sender, EventArgs e)
    {

    }

    private void IrParaMinhasFinancas_Clicked(object sender, EventArgs e)
    {

    }

    private void BTN_ApagarReceitaCategoria_Clicked(object sender, EventArgs e)
    {
        var categoriaSelecionada = PickerCategoriaReset.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(categoriaSelecionada))
        {
            _viewModel.ApagarReceitaPorCategoria(categoriaSelecionada);

            // Atualizar lista após remoção
            _viewModel.ListaReceitas = new ObservableCollection<Receita>(_dbHelpers.GetReceitas());
            OnPropertyChanged(nameof(_viewModel.ListaReceitas)); // Atualiza a interface
        }
        else
        {
            DisplayAlert("Erro", "Selecione uma categoria!", "OK");
        }
    }



    private void BTN_ApagarDespesaCategoria_Clicked(object sender, EventArgs e)
    {
        var categoriaSelecionada = PickerCategoriaExcluir.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(categoriaSelecionada))
        {
            _viewModel.ApagarDespesaPorCategoria(categoriaSelecionada);
        }
    }

}




