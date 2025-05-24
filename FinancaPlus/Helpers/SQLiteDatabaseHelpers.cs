using FinancaPlus.Models;
using FinancaPlus.Views;
using MauiAppFinancaPlus.Moldes;
using SQLite;

namespace FinancaPlus.Helpers
{
    public class SQLiteDatabaseHelpers
    {

        private readonly SQLiteConnection _db;
        private readonly string _dbPath; // Adicionado para armazenar o caminho do banco de dados


        public SQLiteDatabaseHelpers()
        {
            _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "usuario.db3"); // Define o caminho do banco de dados
            _db = new SQLiteConnection(_dbPath);
            _db.CreateTable<Usuario>(); // Cria a tabela de usuários
            _db.CreateTable<Despesa>(); // Cria a tabela de despesas
            _db.CreateTable<Receita>(); // Cria a tabela de receitas
            _db.CreateTable<Categoria>(); // Cria a tabela de categorias
            _db.CreateTable<Transacao>();
            _db.CreateTable<GastoCategoria>();
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
                throw new ArgumentException("Usuário inválido!");


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

        public void DeleteAllReceitas()
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.DeleteAll<Receita>(); // Apaga todos os registros da tabela Receita
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
        public void ResetarSaldoPorCategoria(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            var receitas = connection.Table<Receita>().Where(r => r.Categoria == categoria).ToList();
            foreach (var receita in receitas)
            {
                receita.Valor = 0;
                connection.Update(receita);
            }
        }
        public void DeleteReceitasPorCategoria(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Table<Receita>().Delete(r => r.Categoria == categoria);
        }


        // Adicionando o método GetDespesas
        public List<Despesa> GetDespesas()
        {
            return _db.Table<Despesa>().ToList();
        }

        // Adicionando o método AddDespesa
        public void AddDespesa(Despesa despesa)
        {
            _db.Insert(despesa);
        }

        public List<Categoria> GetCategoriasFixas()
        {
            using var conexao = new SQLiteConnection(_dbPath);
            return conexao.Query<Categoria>("SELECT * FROM Categorias WHERE Tipo = 'Fixa'");
        }

        public List<Categoria> GetCategoriasVariaveis()
        {
            using var conexao = new SQLiteConnection(_dbPath);
            return conexao.Query<Categoria>("SELECT * FROM Categorias WHERE Tipo = 'Variável'");
        }

    }
}

