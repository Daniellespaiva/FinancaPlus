using FinancaPlus.Models;
using MauiAppFinancaPlus.Moldes;
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
            _db.CreateTable<Usuario>(); // Certifique-se de que a classe Usuario está definida no namespace correto
            _db.CreateTable<Receita>(); // Criando a tabela de receitas
        }

        // Adicionar usuário com senha criptografada
        public void AddUsuario(Usuario usuario, string senha)
        {
            if (_db == null)
                throw new InvalidOperationException("Banco de dados não inicializado.");

            if (usuario == null || string.IsNullOrEmpty(senha))
                throw new ArgumentException("Usuário ou senha inválidos.");

            try
            {
                usuario.DefinirSenha(senha); // Criar hash seguro
                _db.Insert(usuario); // Salvar no banco
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar usuário: {ex.Message}");
            }
        }

        // Verificar login do usuário
        public bool VerificarLogin(string email, string senhaDigitada)
        {
            if (_db == null)
                throw new InvalidOperationException("Banco de dados não inicializado.");

            var usuario = GetUsuario(email);
            return usuario != null && usuario.VerificarSenha(senhaDigitada);
        }

        // Buscar usuário por e-mail
        public Usuario? GetUsuario(string email)
        {
            if (_db == null)
                throw new InvalidOperationException("Banco de dados não inicializado.");

            if (string.IsNullOrWhiteSpace(email))
                return null; // Evita erro por campo vazio

            try
            {
                return _db.Table<Usuario>().FirstOrDefault(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar usuário: {ex.Message}");
                return null;
            }
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

        // Adicionando o método GetDespesas
        public List<Gasto> GetDespesas()
        {
            return _db.Table<Gasto>().ToList();
        }

        // Adicionando o método AddDespesa
        public void AddDespesa(Gasto despesa)
        {
            _db.Insert(despesa);
        }

        // Outros métodos existentes...

        public void RemoverReceita(Receita receita)
        {
            if (_db == null)
                throw new InvalidOperationException("Banco de dados não inicializado.");

            if (receita == null)
                throw new ArgumentException("Receita inválida!");

            _db.Delete(receita);
        }

        // Atualizar senha do usuário
        public void AtualizarSenha(string email, string novaSenha)
        {
            if (_db == null)
                throw new InvalidOperationException("Banco de dados não inicializado.");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Email ou senha inválidos!");

            try
            {
                var usuario = GetUsuario(email);
                if (usuario == null)
                    throw new ArgumentException("Usuário não encontrado.");

                usuario.DefinirSenha(novaSenha); // Garante que o hash será gerado corretamente
                _db.Update(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar senha: {ex.Message}");
            }
        }
    }
}

