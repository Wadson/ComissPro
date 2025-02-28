using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ComissPro
{
    internal class ProdutoDAL
    {
        public DataTable listarProduto()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();
                SQLiteCommand sqlcomando = new SQLiteCommand("SELECT * FROM Produtos", conn);
                SQLiteDataAdapter da = new SQLiteDataAdapter();
                da.SelectCommand = sqlcomando;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception erro)
            {
                throw erro;
            }
            finally
            {
                conn.Close();
            }
        }
       
        public void Inserir(Model.ProdutoMODEL produto)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "INSERT INTO Produtos (NomeProduto, Preco, Tipo, QuantidadePorBloco) VALUES (@NomeProduto, @Preco, @Tipo, @QuantidadePorBloco)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@Tipo", produto.Tipo);
                    cmd.Parameters.AddWithValue("@QuantidadePorBloco", produto.QuantidadePorBloco);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void Alterar(Model.ProdutoMODEL produto)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "UPDATE Produtos SET NomeProduto =@NomeProduto, Preco = @Preco, Tipo = @Tipo WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@Tipo", produto.Tipo);
                    cmd.Parameters.AddWithValue("@ProdutoID", produto.ProdutoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Excluir(Model.ProdutoMODEL objetomodel)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "DELETE FROM Produtos WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", objetomodel.ProdutoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public DataTable PesquisarPorNome(string nome)
        {
            var conn = Conexao.Conex();
            try
            {
                DataTable dt = new DataTable();
                string sql = "SELECT ProdutoID, NomeProduto, Preco, Tipo, QuantidadePorBloco FROM Produtos WHERE NomeProduto LIKE @NomeProduto";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@NomeProduto", "%" + nome + "%");
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
        public DataTable PesquisarPorCodigo(string nome)
        {
            var conn = Conexao.Conex();
            try
            {
                DataTable dt = new DataTable();
                string sql = "SELECT ProdutoID, NomeProduto, Preco, Tipo, QuantidadePorBloco FROM Produtos WHERE ProdutoID LIKE @Nome";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@NomeProduto", "%" + nome + "%");
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
