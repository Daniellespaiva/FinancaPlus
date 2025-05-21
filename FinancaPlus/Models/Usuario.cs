using SQLite;
using System.Security.Cryptography;
using System.Text;

namespace MauiAppFinancaPlus.Moldes
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty; // Salt para reforçar segurança

        public Usuario() { }

        // Gerar um salt aleatório
        private static string GerarSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Definir senha segura com PBKDF2
        public void DefinirSenha(string senha)
        {
            Salt = GerarSalt(); // Salt único para cada usuário
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, Convert.FromBase64String(Salt), 100000, HashAlgorithmName.SHA256))
            {
                SenhaHash = Convert.ToBase64String(pbkdf2.GetBytes(32)); // Hash seguro
            }
        }

        // Verificar senha usando PBKDF2
        public bool VerificarSenha(string senhaDigitada)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(senhaDigitada, Convert.FromBase64String(Salt), 100000, HashAlgorithmName.SHA256))
            {
                string senhaDigitadaHash = Convert.ToBase64String(pbkdf2.GetBytes(32));
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(SenhaHash),
                    Encoding.UTF8.GetBytes(senhaDigitadaHash)
                );
            }
        }
    }
}


