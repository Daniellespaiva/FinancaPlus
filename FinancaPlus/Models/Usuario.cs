using SQLite;

namespace FinancaPlus.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;

        public Usuario() { }

        // Método para definir senha com hashing
        public void DefinirSenha(string senha)
        {  //Foi instalodo uma biblioteca BCrypt para protege o banco de dados contra ataques
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        }

        // Método para verificar senha
        public bool VerificarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, SenhaHash);
        }
    }
}



