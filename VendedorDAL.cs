using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ComissPro
{
    internal class VendedorDAL
    {
        public DataTable listaVendedor()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();
                SQLiteCommand sqlcomando = new SQLiteCommand("SELECT * FROM Vendedores", conn);
                SQLiteDataAdapter daFornecedor = new SQLiteDataAdapter();
                daFornecedor.SelectCommand = sqlcomando;
                DataTable dtFornecedor = new DataTable();
                daFornecedor.Fill(dtFornecedor);
                return dtFornecedor;
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
        public void Inserir(Model.VendedorMODEL vendedor)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "INSERT INTO Vendedores (Nome, CPF, Telefone, Comissao) VALUES (@Nome, @CPF, @Telefone, @Comissao)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", vendedor.Nome);
                    cmd.Parameters.AddWithValue("@CPF", vendedor.CPF);
                    cmd.Parameters.AddWithValue("@Telefone", vendedor.Telefone);
                    cmd.Parameters.AddWithValue("@Comissao", vendedor.Comissao);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Alterar(Model.VendedorMODEL vendedor)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "UPDATE Vendedores SET Nome=@Nome, CPF=@CPF, Telefone=@Telefone, Comissao=@Comissao WHERE VendedorID=@VendedorID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", vendedor.Nome);
                    cmd.Parameters.AddWithValue("@CPF", vendedor.CPF);
                    cmd.Parameters.AddWithValue("@Telefone", vendedor.Telefone);
                    cmd.Parameters.AddWithValue("@Comissao", vendedor.Comissao);
                    cmd.Parameters.AddWithValue("@VendedorID", vendedor.VendedorID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Excluir(Model.VendedorMODEL vendedor)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "DELETE FROM Vendedores WHERE VendedorID=@VendedorID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", vendedor.VendedorID);
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
                string sqlconn = "SELECT VendedorID, Nome, CPF, Telefone, Comissao FROM Vendedores WHERE Nome LIKE @Nome";
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
        public DataTable PesquisarPorCodigo(string codigo)
        {
            var conn = Conexao.Conex();
            try
            {
                DataTable dt = new DataTable();
                string sqlconn = "SELECT VendedorID, Nome, CPF, Telefone, Comissao FROM Vendedores WHERE VendedorID LIKE @Codigo";
                SQLiteCommand cmd = new SQLiteCommand(sqlconn, conn);
                cmd.Parameters.AddWithValue("@Codigo", "%" + codigo + "%");
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
