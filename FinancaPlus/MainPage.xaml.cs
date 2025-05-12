using FinancaPlus.Views;

namespace FinancaPlus
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Application.Current.MainPage = new NavigationPage(new TelaPrincipal("email@exemplo.com")); // Ativa a navegação
        }

    }


}
