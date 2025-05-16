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
            _db.CreateTable<Receita>(); // Criando a tabela de receitas
        }        

        // Adicionar usuário com senha criptografada
        public void AddUsuario(Usuario usuario, string senha)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Usuário ou senha inválidos!");

            usuario.DefinirSenha(senha); // Criptografa a senha antes de salvar
            _db.Insert(usuario);

        }

        // Verificar login do usuário
        public Usuario? GetUsuario(string email, string senha)
        {
            if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                return null; // Evita erro por campos vazios

            var usuario = _db.Table<Usuario>().FirstOrDefault(u => u.Email == email);

            return (usuario != null && usuario.VerificarSenha(senha)) ? usuario : null;
        }


        // Buscar usuário por e-mail
        public Usuario? GetUsuario(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null; // Evita erro por campo vazio

            return _db.Table<Usuario>().FirstOrDefault(u => u.Email == email);
        }


        // Atualizar usuário (alterar nome, email, etc.)
        public void UpdateUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new  ArgumentException("Usuário inválido!");


            _db.Update(usuario);
        }
        // Adicionar Receita ao banco de dados
        public void AddReceita(Receita receita)
        {
            if (receita == null)
                throw new ArgumentException("Receita inválida!");

            _db.Insert(receita);
        }

        // Listar todas as receitas armazenadas
        public List<Receita> GetReceitas()
        {
            return _db.Table<Receita>().ToList();
        }
    



        // Atualizar senha do usuário
        public void UpdateSenha(string email, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Email ou senha inválidos!");

            var usuario = GetUsuario(email);
            if (usuario != null)
            {
                usuario.DefinirSenha(novaSenha); // Garante que o hash será gerado corretamente
                _db.Update(usuario);
            }
        }

    }

}
    
