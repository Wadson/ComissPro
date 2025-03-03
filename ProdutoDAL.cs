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
        // Em ProdutoDal
        public Model.ProdutoMODEL BuscarPorId(int produtoId)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT * FROM Produtos WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", produtoId);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Model.ProdutoMODEL
                            {
                                ProdutoID = reader.GetInt32(0),
                                NomeProduto = reader.GetString(1),
                                Preco = reader.GetDouble(2),
                                Unidade = reader.GetString(3)                                
                            };
                        }
                    }
                }
            }
            return null;
        }
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

        public void Salvar(Model.ProdutoMODEL produto)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "INSERT INTO Produtos (NomeProduto, Preco, Unidade) VALUES (@NomeProduto, @Preco, @Unidade)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@Unidade", produto.Unidade);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Novo método para verificar duplicata
        public bool ProdutoExiste(string nomeProduto)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT COUNT(*) FROM Produtos WHERE NomeProduto = @NomeProduto";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", nomeProduto);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Retorna true se o produto já existe
                }
            }
        }


        public void Alterar(Model.ProdutoMODEL produto)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "UPDATE Produtos SET NomeProduto =@NomeProduto, Preco = @Preco, Unidade = @Unidade WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@NomeProduto", produto.NomeProduto);
                    cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@Unidade", produto.Unidade);
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
                string sql = "SELECT ProdutoID, NomeProduto, Preco, Unidade FROM Produtos WHERE NomeProduto LIKE @NomeProduto";
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
