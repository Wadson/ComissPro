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
using System.Runtime.Remoting.Contexts;

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
        public void Salvar(Model.VendedorMODEL vendedor)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message);
                throw;
            }
        }

        // Verifica se o nome já existe
        public bool NomeExiste(string nome)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT COUNT(*) FROM Vendedores WHERE Nome = @Nome";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Verifica se o telefone já existe
        public bool TelefoneExiste(string telefone)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT COUNT(*) FROM Vendedores WHERE Telefone = @Telefone";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Telefone", telefone);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Busca o nome do vendedor associado a um telefone
        public string BuscarNomePorTelefone(string telefone)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT Nome FROM Vendedores WHERE Telefone = @Telefone LIMIT 1";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Telefone", telefone);
                    var resultado = cmd.ExecuteScalar();
                    return resultado != null ? resultado.ToString() : null;
                }
            }
        }

        public string BuscarNomeParecido(string nome)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT Nome FROM Vendedores WHERE REPLACE(Nome, ' ', '') LIKE '%' || REPLACE(@Nome, ' ', '') || '%' OR REPLACE(@Nome, ' ', '') LIKE '%' || REPLACE(Nome, ' ', '') || '%'";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    var resultado = cmd.ExecuteScalar();
                    return resultado != null ? resultado.ToString() : null;
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

        //public void Excluir(Model.VendedorMODEL vendedor)
        //{
        //    using (var conexao = Conexao.Conex())
        //    {
        //        conexao.Open();
        //        string sql = "DELETE FROM Vendedores WHERE VendedorID=@VendedorID";
        //        using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
        //        {
        //            cmd.Parameters.AddWithValue("@VendedorID", vendedor.VendedorID);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
        public int ContarEntregasPendentes(int vendedorID)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "SELECT COUNT(*) FROM Entregas WHERE VendedorID = @VendedorID AND PrestacaoRealizada = 0";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", vendedorID);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void Excluir(int vendedorID)
        {
            try
            {
                using (var conexao = Conexao.Conex())
                {
                    conexao.Open();
                    string sql = "DELETE FROM Vendedores WHERE VendedorID = @VendedorID";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                    {
                        cmd.Parameters.AddWithValue("@VendedorID", vendedorID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Nenhum vendedor foi excluído.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir vendedor: " + ex.Message);
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
                v.VendedorID,
                v.Nome AS NomeVendedor,
                pc.Comissao,
                pc.DataPrestacao,
                pc.QuantidadeVendida,
                pc.QuantidadeDevolvida,
                pc.ValorRecebido,
                pc.PrestacaoID,
                pc.EntregaID
            FROM 
                PrestacaoContas pc
            INNER JOIN 
                Entregas e ON pc.EntregaID = e.EntregaID
            INNER JOIN 
                Vendedores v ON e.VendedorID = v.VendedorID
            WHERE 
                1 = 1"; // Condição base para filtros opcionais

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
                                    VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                    NomeVendedor = reader["NomeVendedor"].ToString(),
                                    Comissao = Convert.ToDouble(reader["Comissao"]),
                                    DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"]),
                                    QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                    QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                    ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                    PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                    EntregaID = Convert.ToInt32(reader["EntregaID"])
                                });
                            }
                        }
                    }
                }
                return prestacoes;
            }

            // Outros métodos como PesquisarEntrega e CarregarEntregasNaoPrestadas permanecem iguais
        }

        // Novo: Relatório de Entregas Pendentes por Vendedor
        public List<EntregasModel> RelatorioEntregasPendentes(string nomeVendedor = null)
        {
            List<EntregasModel> entregas = new List<EntregasModel>();
            string query = @"
            SELECT 
                v.VendedorID,
                v.Nome AS NomeVendedor,
                e.EntregaID,
                e.QuantidadeEntregue,
                e.DataEntrega,
                p.NomeProduto
            FROM 
                Entregas e
            INNER JOIN 
                Vendedores v ON e.VendedorID = v.VendedorID
            INNER JOIN 
                Produtos p ON e.ProdutoID = p.ProdutoID
            WHERE 
                e.PrestacaoRealizada = 0";

            if (!string.IsNullOrEmpty(nomeVendedor))
                query += " AND v.Nome LIKE @NomeVendedor";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(nomeVendedor))
                        cmd.Parameters.AddWithValue("@NomeVendedor", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entregas.Add(new EntregasModel
                            {
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                NomeVendedor = reader["NomeVendedor"].ToString(),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
                                NomeProduto = reader["NomeProduto"].ToString()
                            });
                        }
                    }
                }
            }
            return entregas;
        }

        // Novo: Relatório de Desempenho de Vendas
        public List<DesempenhoVendasModel> RelatorioDesempenhoVendas(DateTime? dataInicio = null, DateTime? dataFim = null, string nomeVendedor = null)
        {
            List<DesempenhoVendasModel> desempenho = new List<DesempenhoVendasModel>();
            string query = @"
            SELECT 
                v.VendedorID,
                v.Nome AS NomeVendedor,
                e.EntregaID,
                e.QuantidadeEntregue,
                COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida,
                COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida,
                COALESCE(pc.ValorRecebido, 0) AS ValorRecebido,
                COALESCE(pc.Comissao, 0) AS Comissao
            FROM 
                Entregas e
            INNER JOIN 
                Vendedores v ON e.VendedorID = v.VendedorID
            LEFT JOIN 
                PrestacaoContas pc ON e.EntregaID = pc.EntregaID
            WHERE 
                1 = 1";

            if (dataInicio.HasValue)
                query += " AND (pc.DataPrestacao >= @DataInicio OR pc.DataPrestacao IS NULL)";
            if (dataFim.HasValue)
                query += " AND (pc.DataPrestacao <= @DataFim OR pc.DataPrestacao IS NULL)";
            if (!string.IsNullOrEmpty(nomeVendedor))
                query += " AND v.Nome LIKE @NomeVendedor";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@NomeVendedor", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            desempenho.Add(new DesempenhoVendasModel
                            {
                                VendedorID = Convert.ToInt64(reader["VendedorID"]),
                                NomeVendedor = reader["NomeVendedor"].ToString(),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                QuantidadeEntregue = Convert.ToInt64(reader["QuantidadeEntregue"]),
                                QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                Comissao = Convert.ToDouble(reader["Comissao"])
                            });
                        }
                    }
                }
            }
            return desempenho;
        }

        // Novo: Relatório Geral de Vendas e Comissões
        public List<GeralVendasComissoesModel> RelatorioGeralVendasComissoes(DateTime? dataInicio = null, DateTime? dataFim = null, string nomeVendedor = null)
        {
            List<GeralVendasComissoesModel> geral = new List<GeralVendasComissoesModel>();
            string query = @"
            SELECT 
                v.VendedorID,
                v.Nome AS NomeVendedor,
                SUM(e.QuantidadeEntregue) AS TotalEntregue,
                SUM(COALESCE(pc.QuantidadeVendida, 0)) AS TotalVendido,
                SUM(COALESCE(pc.QuantidadeDevolvida, 0)) AS TotalDevolvido,
                SUM(COALESCE(pc.ValorRecebido, 0)) AS TotalRecebido,
                SUM(COALESCE(pc.Comissao, 0)) AS TotalComissao
            FROM 
                Entregas e
            INNER JOIN 
                Vendedores v ON e.VendedorID = v.VendedorID
            LEFT JOIN 
                PrestacaoContas pc ON e.EntregaID = pc.EntregaID
            WHERE 
                1 = 1
            GROUP BY 
                v.VendedorID, v.Nome";

            if (dataInicio.HasValue)
                query = query.Replace("WHERE 1 = 1", "WHERE (pc.DataPrestacao >= @DataInicio OR pc.DataPrestacao IS NULL)");
            if (dataFim.HasValue)
                query += " AND (pc.DataPrestacao <= @DataFim OR pc.DataPrestacao IS NULL)";
            if (!string.IsNullOrEmpty(nomeVendedor))
                query += " AND v.Nome LIKE @NomeVendedor";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@NomeVendedor", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            geral.Add(new GeralVendasComissoesModel
                            {
                                VendedorID = Convert.ToInt64(reader["VendedorID"]),
                                NomeVendedor = reader["NomeVendedor"].ToString(),
                                TotalEntregue = Convert.ToInt64(reader["TotalEntregue"]),
                                TotalVendido = Convert.ToInt64(reader["TotalVendido"]),
                                TotalDevolvido = Convert.ToInt64(reader["TotalDevolvido"]),
                                TotalRecebido = Convert.ToDouble(reader["TotalRecebido"]),
                                TotalComissao = Convert.ToDouble(reader["TotalComissao"])
                            });
                        }
                    }
                }
            }
            return geral;
        }



    }
}
