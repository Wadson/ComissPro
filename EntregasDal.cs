using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;

namespace ComissPro
{
    internal class EntregasDal
    {

        // Em EntregasDal
        public void Inserir(Model.EntregasModel entrega)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "INSERT INTO Entregas (VendedorID, ProdutoID, QuantidadeEntregue, DataEntrega, ValorTotal) " +
                             "VALUES (@VendedorID, @ProdutoID, @QuantidadeEntregue, @DataEntrega, @ValorTotal)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", entrega.VendedorID);
                    cmd.Parameters.AddWithValue("@ProdutoID", entrega.ProdutoID);
                    cmd.Parameters.AddWithValue("@QuantidadeEntregue", entrega.QuantidadeEntregue);
                    cmd.Parameters.AddWithValue("@DataEntrega", entrega.DataEntrega);                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable listaEntregas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();

                string query = @"
            SELECT 
                e.EntregaID, 
                e.VendedorID, 
                COALESCE(v.Nome, 'Vendedor Excluído') AS NomeVendedor, 
                e.ProdutoID, 
                p.NomeProduto, 
                e.QuantidadeEntregue, 
                e.DataEntrega, 
                e.PrestacaoRealizada,
                (e.QuantidadeEntregue * COALESCE(p.Preco, 0)) AS Total
            FROM Entregas e
            LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
            LEFT JOIN Produtos p ON e.ProdutoID = p.ProdutoID
            WHERE e.PrestacaoRealizada = 0;";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
                SQLiteDataAdapter daFornecedor = new SQLiteDataAdapter();
                daFornecedor.SelectCommand = sqlcomando;
                DataTable dtFornecedor = new DataTable();
                daFornecedor.Fill(dtFornecedor);

                // Calcular totais
                if (dtFornecedor.Rows.Count > 0)
                {
                    long totalQuantidadeEntregue = dtFornecedor.AsEnumerable()
                        .Sum(row => row["QuantidadeEntregue"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeEntregue"]));
                    double totalTotal = dtFornecedor.AsEnumerable()
                        .Sum(row => row["Total"] is DBNull ? 0.0 : Convert.ToDouble(row["Total"]));

                    // Adicionar linha de totais com valores padrão para todas as colunas
                    DataRow totalRow = dtFornecedor.NewRow();
                    totalRow["EntregaID"] = DBNull.Value; // ou 0, dependendo do seu uso
                    totalRow["VendedorID"] = DBNull.Value; // ou -1 para indicar "sem vendedor"
                    totalRow["NomeVendedor"] = "Totais";
                    totalRow["ProdutoID"] = DBNull.Value; // ou -1
                    totalRow["NomeProduto"] = DBNull.Value; // ou "N/A"
                    totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue;
                    totalRow["DataEntrega"] = DBNull.Value; // ou DateTime.Now, se preferir
                    totalRow["PrestacaoRealizada"] = DBNull.Value; // ou 0
                    totalRow["Total"] = totalTotal;
                    dtFornecedor.Rows.Add(totalRow);
                }

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
        public List<EntregasModel> CarregarEntregasNaoPrestadas()
        {
            List<EntregasModel> entregas = new List<EntregasModel>();
            string query = @"
            SELECT
                Vendedores.Nome AS NomeVendedor,
                Produtos.NomeProduto,
                Entregas.QuantidadeEntregue,
                Produtos.Preco,
                (Entregas.QuantidadeEntregue * Produtos.Preco) AS Total,
                Entregas.DataEntrega,
                Entregas.EntregaID,
                Entregas.VendedorID,
                Entregas.ProdutoID,
                Vendedores.Comissao
            FROM
                Entregas
            INNER JOIN
                Vendedores ON Entregas.VendedorID = Vendedores.VendedorID
            INNER JOIN
                Produtos ON Entregas.ProdutoID = Produtos.ProdutoID
            WHERE
                Entregas.PrestacaoRealizada = 0";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entregas.Add(new EntregasModel
                            {
                                NomeVendedor = reader["NomeVendedor"].ToString(),
                                NomeProduto = reader["NomeProduto"].ToString(),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                Preco = Convert.ToDouble(reader["Preco"]),
                                Total = Convert.ToDouble(reader["Total"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                ProdutoID = Convert.ToInt32(reader["ProdutoID"]),
                                Comissao = Convert.ToDouble(reader["Comissao"]) // Percentual direto (ex.: 40)
                            });
                        }
                    }
                }
            }
            return entregas;
        }
        public void SalvarEntregas(Model.EntregasModel entrega)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"INSERT INTO Entregas (VendedorID, ProdutoID, QuantidadeEntregue, DataEntrega, PrestacaoRealizada) 
                            VALUES (@VendedorID, @ProdutoID, @QuantidadeEntregue, @DataEntrega, @PrestacaoRealizada)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", entrega.VendedorID);
                    cmd.Parameters.AddWithValue("@ProdutoID", entrega.ProdutoID);
                    cmd.Parameters.AddWithValue("@QuantidadeEntregue", entrega.QuantidadeEntregue);
                    cmd.Parameters.AddWithValue("@DataEntrega", entrega.DataEntrega);
                    cmd.Parameters.AddWithValue("@PrestacaoRealizada", entrega.PrestacaoRealizada ? 1 : 0); // 0 para nova entrega
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AlterarEntrega(EntregasModel entrega)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "UPDATE Entregas SET VendedorID = @VendedorID, ProdutoID = @ProdutoID, QuantidadeEntregue = @QuantidadeEntregue, DataEntrega = @DataEntrega WHERE EntregaID = @EntregaID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EntregaID", entrega.EntregaID);
                    cmd.Parameters.AddWithValue("@VendedorID", entrega.VendedorID);
                    cmd.Parameters.AddWithValue("@ProdutoID", entrega.ProdutoID);
                    cmd.Parameters.AddWithValue("@QuantidadeEntregue", entrega.QuantidadeEntregue);
                    cmd.Parameters.AddWithValue("@DataEntrega", entrega.DataEntrega);
                    cmd.ExecuteNonQuery();
                }
            }
        }
               
        public void Excluir(Model.EntregasModel entrega)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "DELETE FROM Entregas WHERE EntregaID = @EntregaID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@EntregaID", entrega.EntregaID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<EntregasModel> PesquisarEntrega(string pesquisa)
        {
            List<EntregasModel> entregas = new List<EntregasModel>();

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                SELECT
                    Vendedores.Nome AS NomeVendedor,
                    Produtos.NomeProduto,
                    Entregas.QuantidadeEntregue,
                    Produtos.Preco,
                    (Entregas.QuantidadeEntregue * Produtos.Preco) AS Total,
                    Entregas.DataEntrega,
                    Entregas.EntregaID,
                    Entregas.VendedorID,
                    Entregas.ProdutoID
                FROM
                    Entregas
                INNER JOIN
                    Vendedores ON Entregas.VendedorID = Vendedores.VendedorID
                INNER JOIN
                    Produtos ON Entregas.ProdutoID = Produtos.ProdutoID
                WHERE 
                    Vendedores.Nome LIKE @Pesquisa";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pesquisa", "%" + pesquisa + "%"); // Pesquisa parcial com LIKE

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entregas.Add(new EntregasModel
                            {
                                NomeVendedor = reader["NomeVendedor"].ToString(),
                                NomeProduto = reader["NomeProduto"].ToString(),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                Preco = Convert.ToDouble(reader["Preco"]),
                                Total = Convert.ToDouble(reader["Total"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                ProdutoID = Convert.ToInt32(reader["ProdutoID"])
                                // PrestacaoRealizada não está na query; se precisar, adicione ao SELECT
                            });
                        }
                    }
                }
            }

            return entregas;
        }


        // Relatório já existente: Comissões Pagas por Vendedor
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
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@NomeVendedor", "%" + nomeVendedor + "%");

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
