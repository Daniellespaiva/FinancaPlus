using FinancaPlus.Models;
using FinancaPlus.Views;
using MauiAppFinancaPlus.Moldes;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            _db.CreateTable<Transacao>();
            _db.CreateTable<GastoCategoria>();
            _db.CreateTable<CategoriaDespesa>(); // Cria a tabela de categorias
            _db.CreateTable<Meta>(); // Cria a tabela de metas
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

        public List<CategoriaDespesa> GetCategoriasFixas()
        {
            using var conexao = new SQLiteConnection(_dbPath);
            return conexao.Query<CategoriaDespesa>("SELECT * FROM Categorias WHERE Tipo = 'Fixa'");
        }

        public List<CategoriaDespesa> GetCategoriasVariaveis()
        {
            using var conexao = new SQLiteConnection(_dbPath);
            return conexao.Query<CategoriaDespesa>("SELECT * FROM Categorias WHERE Tipo = 'Variável'");
        }

        public void DeleteCategoriaReceita(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("DELETE FROM Receita WHERE Categoria = ?", categoria);
        }

        public void DeleteReceitasPorCategoria(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("DELETE FROM Receita WHERE Categoria = ?", categoria);
        }

        public List<Meta> GetMetas()
        {
            using var connection = new SQLiteConnection(_dbPath);
            return connection.Table<Meta>().ToList();
        }
        public void AddMeta(Meta meta)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Insert(meta);
        }
        public void DeleteMetasPorCategoria(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("DELETE FROM Meta WHERE Categoria = ?", categoria);
        }

        public void ResetarSaldoPorCategoria(string categoria)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("UPDATE Meta SET Valor = 0 WHERE Categoria = ?", categoria);
        }

        public void ExcluirTransacao(Transacao transacao)
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Delete(transacao);
        }

        public void ResetarSaldo()
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("UPDATE Receita SET Valor = 0");
        }

        public void ResetarDespesas()
        {
            using var connection = new SQLiteConnection(_dbPath);
            connection.Execute("UPDATE Despesa SET Valor = 0");
        }
        public decimal ObterTotalDespesas()
        {
            using var connection = new SQLiteConnection(_dbPath);
            var resultado = connection.ExecuteScalar<decimal>("SELECT SUM(Valor) FROM Despesa");
            return resultado;
        }

        public decimal ObterTotalReceita()
        {
            using var connection = new SQLiteConnection(_dbPath);
            var resultado = connection.ExecuteScalar<decimal>("SELECT COALESCE(SUM(Valor), 0) FROM Receita");
            return resultado;
        }
    }

}

