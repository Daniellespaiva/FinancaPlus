using SQLite;

namespace FinancaPlus.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
               

        public Usuario() { }

        // Método para definir senha com hashing seguro
        public void DefinirSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("A senha não pode estar vazia.");

            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha); // Agora criptografa corretamente
        }

        // Método para verificar senha
        public bool VerificarSenha(string senhaDigitada)
        {
            if (string.IsNullOrWhiteSpace(senhaDigitada) || string.IsNullOrWhiteSpace(SenhaHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(senhaDigitada, SenhaHash);
        }
    }

}

        




