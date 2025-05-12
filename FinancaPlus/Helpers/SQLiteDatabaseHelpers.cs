using FinancaPlus.Models;
using SQLite;

namespace FinancaPlus.Helpers
{
    public class SQLiteDatabaseHelpers
    {
        private readonly SQLiteConnection _db;

        public SQLiteDatabaseHelpers()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "usuarios.db3");
            _db = new SQLiteConnection(path);
            _db.CreateTable<Usuario>();
        }

        // Adicionar usuário com senha criptografada
        public void AddUsuario(Usuario usuario, string senha)
        {
            usuario.DefinirSenha(senha); // Criptografando a senha antes de salvar
            _db.Insert(usuario);
        }

        // Verificar login do usuário
        public Usuario GetUsuario(string email, string senha)
        {
            var usuario = _db.Table<Usuario>().FirstOrDefault(u => u.Email == email);

            if (usuario != null && usuario.VerificarSenha(senha))
                return usuario;

            return null; // Retorna null se o login falhar
        }

        // Buscar usuário por e-mail
        public Usuario GetUsuario(string email)
        {
            return _db.Table<Usuario>().FirstOrDefault(u => u.Email == email);
        }

        // Atualizar usuário (alterar nome, email, etc.)
        public void UpdateUsuario(Usuario usuario)
        {
            _db.Update(usuario);
        }

        // Atualizar senha do usuário
        public void UpdateSenha(string email, string novaSenha)
        {
            var usuario = GetUsuario(email);
            if (usuario != null)
            {
                usuario.DefinirSenha(novaSenha); // Criptografando a nova senha
                _db.Update(usuario);
            }
        }
    }
}