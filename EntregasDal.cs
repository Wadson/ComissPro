﻿using DocumentFormat.OpenXml.Vml;
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
                v.Nome, 
                e.ProdutoID, 
                p.NomeProduto, 
                e.QuantidadeEntregue,
                e.DataEntrega, 
                e.PrestacaoRealizada, 
                p.Preco,
                e.QuantidadeEntregue * p.Preco AS Total
            FROM Entregas e
            JOIN Vendedores v ON e.VendedorID = v.VendedorID
            JOIN Produtos p ON e.ProdutoID = p.ProdutoID
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
                    totalRow["EntregaID"] = DBNull.Value;
                    totalRow["VendedorID"] = DBNull.Value;
                    totalRow["Nome"] = "Totais";
                    totalRow["ProdutoID"] = DBNull.Value;
                    totalRow["NomeProduto"] = DBNull.Value;
                    totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue;
                    totalRow["DataEntrega"] = DBNull.Value;
                    totalRow["PrestacaoRealizada"] = DBNull.Value;
                    totalRow["Preco"] = DBNull.Value; // Não soma Preço, deixa vazio
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
        // Novo método para excluir entregas órfãs
        public int ExcluirEntregasOrfas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();

                string query = @"
                DELETE FROM Entregas 
                WHERE VendedorID NOT IN (SELECT VendedorID FROM Vendedores);";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
                int rowsAffected = sqlcomando.ExecuteNonQuery();

                return rowsAffected; // Retorna o número de entregas excluídas
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao excluir entregas órfãs: " + erro.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public List<EntregasModel> CarregarEntregasNaoPrestadas(string filtro = "")
        {
            List<EntregasModel> entregas = new List<EntregasModel>();
            string query = @"
    SELECT
        Vendedores.Nome AS Nome,
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

            // Adicionar filtro caso tenha sido informado
            if (!string.IsNullOrEmpty(filtro))
            {
                query += " AND (Vendedores.Nome LIKE @filtro OR Produtos.NomeProduto LIKE @filtro)";
            }

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
                    }

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entregas.Add(new EntregasModel
                            {
                                Nome = reader["Nome"].ToString(),
                                NomeProduto = reader["NomeProduto"].ToString(),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                Preco = Convert.ToDouble(reader["Preco"]),
                                Total = Convert.ToDouble(reader["Total"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                ProdutoID = Convert.ToInt32(reader["ProdutoID"]),
                                Comissao = Convert.ToDouble(reader["Comissao"])
                            });
                        }
                    }
                }
            }
            return entregas;
        }

        //public List<EntregasModel> CarregarEntregasNaoPrestadas()
        //{
        //    List<EntregasModel> entregas = new List<EntregasModel>();
        //    string query = @"
        //    SELECT
        //        Vendedores.Nome AS Nome,
        //        Produtos.NomeProduto,
        //        Entregas.QuantidadeEntregue,
        //        Produtos.Preco,
        //        (Entregas.QuantidadeEntregue * Produtos.Preco) AS Total,
        //        Entregas.DataEntrega,
        //        Entregas.EntregaID,
        //        Entregas.VendedorID,
        //        Entregas.ProdutoID,
        //        Vendedores.Comissao
        //    FROM
        //        Entregas
        //    INNER JOIN
        //        Vendedores ON Entregas.VendedorID = Vendedores.VendedorID
        //    INNER JOIN
        //        Produtos ON Entregas.ProdutoID = Produtos.ProdutoID
        //    WHERE
        //        Entregas.PrestacaoRealizada = 0";

        //    using (var conn = Conexao.Conex())
        //    {
        //        conn.Open();
        //        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    entregas.Add(new EntregasModel
        //                    {
        //                        Nome = reader["Nome"].ToString(),
        //                        NomeProduto = reader["NomeProduto"].ToString(),
        //                        QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
        //                        Preco = Convert.ToDouble(reader["Preco"]),
        //                        Total = Convert.ToDouble(reader["Total"]),
        //                        DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
        //                        EntregaID = Convert.ToInt32(reader["EntregaID"]),
        //                        VendedorID = Convert.ToInt32(reader["VendedorID"]),
        //                        ProdutoID = Convert.ToInt32(reader["ProdutoID"]),
        //                        Comissao = Convert.ToDouble(reader["Comissao"]) // Percentual direto (ex.: 40)
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return entregas;
        //}
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
                    cmd.Parameters.AddWithValue("@PrestacaoRealizada", entrega.Prestacaorealizada ? 1 : 0); // 0 para nova entrega
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AlterarEntrega(Model.EntregasModel entrega)
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
                    Vendedores.Nome AS Nome,
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
                                Nome = reader["Nome"].ToString(),
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
            v.Nome AS Nome,
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
                query += " AND v.Nome LIKE @Nome";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@Nome", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prestacoes.Add(new PrestacaoContasModel
                            {
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                Nome = reader["Nome"].ToString(),
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

        public List<EntregasModel> RelatorioEntregasPendentes(string nomeVendedor = null)
        {
            List<EntregasModel> entregas = new List<EntregasModel>();
            string query = @"
        SELECT 
            v.VendedorID,
            v.Nome AS Nome,
            e.EntregaID,
            e.QuantidadeEntregue,
            e.DataEntrega,
            p.NomeProduto,
            p.Preco,
            p.ProdutoID,
            COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida, -- Adicionado
            COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida, -- Adicionado
            COALESCE(pc.ValorRecebido, 0) AS ValorRecebido -- Adicionado
        FROM 
            Entregas e
        INNER JOIN 
            Vendedores v ON e.VendedorID = v.VendedorID
        INNER JOIN 
            Produtos p ON e.ProdutoID = p.ProdutoID
        LEFT JOIN 
            PrestacaoContas pc ON e.EntregaID = pc.EntregaID -- LEFT JOIN para incluir mesmo sem prestação
        WHERE 
            e.PrestacaoRealizada = 0";

            if (!string.IsNullOrEmpty(nomeVendedor))
                query += " AND v.Nome LIKE @Nome";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(nomeVendedor))
                        cmd.Parameters.AddWithValue("@Nome", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entrega = new EntregasModel
                            {
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                Nome = reader["Nome"].ToString(),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"]),
                                NomeProduto = reader["NomeProduto"].ToString(),
                                Preco = Convert.ToDouble(reader["Preco"]),
                                ProdutoID = Convert.ToInt32(reader["ProdutoID"]),
                                QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]), // Adicionado
                                QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]), // Adicionado
                                ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]) // Adicionado
                            };
                            entrega.Total = entrega.QuantidadeEntregue * entrega.Preco;
                            entregas.Add(entrega);
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
                v.Nome AS Nome,
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
                query += " AND v.Nome LIKE @Nome";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@Nome", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            desempenho.Add(new DesempenhoVendasModel
                            {
                                VendedorID = Convert.ToInt64(reader["VendedorID"]),
                                Nome = reader["Nome"].ToString(),
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
                v.Nome AS Nome,
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
                query += " AND v.Nome LIKE @Nome";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (dataInicio.HasValue) cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
                    if (dataFim.HasValue) cmd.Parameters.AddWithValue("@DataFim", dataFim.Value);
                    if (!string.IsNullOrEmpty(nomeVendedor)) cmd.Parameters.AddWithValue("@Nome", "%" + nomeVendedor + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            geral.Add(new GeralVendasComissoesModel
                            {
                                VendedorID = Convert.ToInt64(reader["VendedorID"]),
                                Nome = reader["Nome"].ToString(),
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
