using FinancaPlus.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancaPlus.Helpers
{
    public class SQLiteDatabaseHelpers
    {
        private readonly SQLiteConnection _db;

        public SQLiteDatabaseHelpers() 
        {
            var path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "usuarios.db3");
            _db = new SQLiteConnection(path);
            _db.CreateTable<Usuario>();
        }
        public void AddUsuario(Usuario usuario) 
        {
            _db.Insert(usuario);
        }
        public Usuario GetUsuario(string email, string senha) 
        {
            return _db.Table<Usuario>().FirstOrDefault(
              u=> u.Email == email && u.Senha == senha);
        }

        public Usuario GetUsuario(string email)
        {
            return _db.Table<Usuario>().FirstOrDefault(
              u => u.Email == email );
        }
        public void UpdateUsuario(Usuario usuario) 
        {
            _db.Update(usuario);
        }

    }
}
