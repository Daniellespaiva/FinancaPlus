using FinancaPlus.Helpers;
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

        // Carrega os dados do banco com conversão de tipos
        CategoriasFixas = new ObservableCollection<Categoria>(
            _dbHelpers.GetCategoriasFixas().Select(c => new Categoria { Nome = c.Nome, Icone = c.Icone })
        );
        CategoriasVariaveis = new ObservableCollection<Categoria>(
            _dbHelpers.GetCategoriasVariaveis().Select(c => new Categoria { Nome = c.Nome, Icone = c.Icone })
        );

        BindingContext = this;
    }

    private async void BTN_AdicionarCategoria_Clicked(object sender, EventArgs e)
    {

    }
}
