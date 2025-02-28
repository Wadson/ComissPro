using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static ComissPro.Model;

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
        //RELATÓRIOS GERENCIAIS

        public class EntregasDAL
        {
            public List<PrestacaoContasModel> RelatorioComissoesPagas(DateTime? dataInicio = null, DateTime? dataFim = null, string nomeVendedor = null)
            {
                List<PrestacaoContasModel> prestacoes = new List<PrestacaoContasModel>();
                string query = @"
            SELECT 
                pc.PrestacaoID,
                pc.EntregaID,
                pc.QuantidadeVendida,
                pc.QuantidadeDevolvida,
                pc.ValorRecebido,
                pc.Comissao,
                pc.DataPrestacao,
                v.Nome AS NomeVendedor,
                v.VendedorID
            FROM 
                PrestacaoContas pc
            INNER JOIN 
                Entregas e ON pc.EntregaID = e.EntregaID
            INNER JOIN 
                Vendedores v ON e.VendedorID = v.VendedorID
            WHERE 
                1 = 1"; // Condição base para filtros opcionais

                // Filtros opcionais
                if (dataInicio.HasValue)
                    query += " AND pc.DataPrestacao >= @DataInicio";
                if (dataFim.HasValue)
                    query += " AND pc.DataPrestacao <= @DataFim";
                if (!string.IsNullOrEmpty(nomeVendedor))
                    query += " AND v.Nome LIKE @NomeVendedor";

                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (dataInicio.HasValue)
                            cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                        if (dataFim.HasValue)
                            cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                        if (!string.IsNullOrEmpty(nomeVendedor))
                            cmd.Parameters.AddWithValue("@NomeVendedor", "%" + nomeVendedor + "%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                prestacoes.Add(new PrestacaoContasModel
                                {
                                    PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                    EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                    QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                    QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                    ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                    Comissao = Convert.ToDouble(reader["Comissao"]),
                                    DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"]),
                                    NomeVendedor = reader["NomeVendedor"].ToString(), // Adicionado ao modelo
                                    VendedorID = Convert.ToInt32(reader["VendedorID"]) // Adicionado ao modelo
                                });
                            }
                        }
                    }
                }
                return prestacoes;
            }

            // Outros métodos como PesquisarEntrega e CarregarEntregasNaoPrestadas permanecem iguais
        }



    }
}
