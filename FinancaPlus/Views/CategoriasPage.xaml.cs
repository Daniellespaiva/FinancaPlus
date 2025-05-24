using FinancaPlus.Helpers;
using FinancaPlus.Models;
using System.Collections.ObjectModel;

namespace FinancaPlus.Views;

public partial class CategoriasPage : ContentPage
{
    private readonly SQLiteDatabaseHelpers _dbHelpers;

    public ObservableCollection<Categoria> CategoriasFixas { get; set; }
    public ObservableCollection<Categoria> CategoriasVariaveis { get; set; }

    public CategoriasPage()
    {
        InitializeComponent();
        _dbHelpers = new SQLiteDatabaseHelpers();

        // Carrega os dados do banco
        CategoriasFixas = new ObservableCollection<Categoria>(_dbHelpers.GetCategoriasFixas());
        CategoriasVariaveis = new ObservableCollection<Categoria>(_dbHelpers.GetCategoriasVariaveis());

        BindingContext = this;
    }

    private async void BTN_AdicionarCategoria_Clicked(object sender, EventArgs e)
    {

    }
}
