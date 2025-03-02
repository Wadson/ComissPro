using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComissPro
{
    internal class UsuarioDAL
    {
        public void Inserir(Model.UsuarioMODEL usuario)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "INSERT INTO Usuarios (Nome, Email, Senha, TipoUsuario) VALUES (@Nome, @Email, @Senha, @TipoUsuario)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
                    cmd.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Alterar(Model.UsuarioMODEL usuario)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "UPDATE Usuarios SET Nome=@Nome, Email=@Email, Senha=@Senha, TipoUsuario=@TipoUsuario WHERE ProdutoID=@ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
                    cmd.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);
                    cmd.Parameters.AddWithValue("@ProdutoID", usuario.UsuarioID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Excluir(Model.UsuarioMODEL usuarios)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "DELETE FROM Usuarios WHERE UsuarioID=@UsuarioID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarios.UsuarioID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método de pesquisa que retorna DataTable, conforme solicitado
        public DataTable PesquisarPorNome(string nome)
        {
            var conn = Conexao.Conex();
            try
            {
                DataTable dt = new DataTable();
                string sqlconn = "SELECT ProdutoID, Nome, Email, Senha, TipoUsuario FROM Usuarios WHERE Nome LIKE @Nome";
                SQLiteCommand cmd = new SQLiteCommand(sqlconn, conn);
                cmd.Parameters.AddWithValue("@Nome", "%" + nome + "%");
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao executar a pesquisa: " + ex.Message);
                return null;
            }
        }
    }
}
