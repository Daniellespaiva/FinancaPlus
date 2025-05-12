namespace FinancaPlus.Models
{
    using Microsoft.Maui.Controls;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class PerfilUsuario : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _nomeUsuario;
        private string _bio;
        private string _email;
        private string _telefone;
        private ImageSource _imagemUsuario;

        public ImageSource ImagemUsuario
        {
            get => _imagemUsuario;
            set
            {
                _imagemUsuario = value;
                OnPropertyChanged();
            }
        }

        public string NomeUsuario
        {
            get => _nomeUsuario;
            set
            {
                _nomeUsuario = value;
                OnPropertyChanged();
            }
        }

        public string Bio
        {
            get => _bio;
            set
            {
                _bio = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Telefone
        {
            get => _telefone;
            set
            {
                _telefone = value;
                OnPropertyChanged();
            }
        }

        public ICommand EditarPerfilCommand { get; }
        public ICommand LogoutCommand { get; }

        public PerfilUsuario()
        {
            // Inicializa os comandos
            EditarPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("editarPerfil"));
            LogoutCommand = new Command(async () => await Shell.Current.GoToAsync("//login"));

            // Carregar dados do usuário
            DadosUsuario();
        }

        private void DadosUsuario()
        {
            // Simulação de busca dos dados do usuário (substituir por banco de dados ou API)
            ImagemUsuario = ImageSource.FromFile("default_profile.png");
            NomeUsuario = "Usuário Teste";
            Bio = "Gosto de desenvolver aplicações!";
            Email = "usuario@email.com";
            Telefone= "(11) 99999-9999";
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
