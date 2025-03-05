using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;
using static ComissPro.Utilitario;

namespace ComissPro
{
    internal class PrestacaoDeContasDAL
    {
        private void Log(string message)
        {
            File.AppendAllText("Log em PrestacaoDeContasDAL.txt", $"{DateTime.Now}: {message}\n");
        }
        public DataTable PesquisaVendasConcluidasPorVendedor(string nomeVendedor = "")
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();
                string query = @"
            SELECT 
                COALESCE(v.Nome, 'Vendedor Excluído') AS Nome,
                COALESCE(e.QuantidadeEntregue, 0) AS QuantidadeEntregue,
                p.NomeProduto,
                COALESCE(p.Preco, 0) AS Preco,
                COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida,
                COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida,
                COALESCE(pc.ValorRecebido, 0) AS ValorRecebido,
                COALESCE(pc.Comissao, 0) AS Comissao,
                pc.DataPrestacao,
                pc.EntregaID,
                pc.PrestacaoID,
                e.VendedorID
            FROM PrestacaoContas pc
            LEFT JOIN Entregas e ON pc.EntregaID = e.EntregaID
            LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
            LEFT JOIN Produtos p ON e.ProdutoID = p.ProdutoID
            WHERE v.Nome LIKE @NomeVendedor || '%'";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
                sqlcomando.Parameters.AddWithValue("@NomeVendedor", nomeVendedor);
                SQLiteDataAdapter da = new SQLiteDataAdapter();
                da.SelectCommand = sqlcomando;
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    long totalQuantidadeEntregue = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeEntregue"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeEntregue"]));
                    long totalQuantidadeVendida = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeVendida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeVendida"]));
                    long totalQuantidadeDevolvida = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeDevolvida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeDevolvida"]));
                    double totalValorRecebido = dt.AsEnumerable()
                        .Sum(row => row["ValorRecebido"] is DBNull ? 0.0 : Convert.ToDouble(row["ValorRecebido"]));
                    double totalComissao = dt.AsEnumerable()
                        .Sum(row => row["Comissao"] is DBNull ? 0.0 : Convert.ToDouble(row["Comissao"]));

                    DataRow totalRow = dt.NewRow();
                    totalRow["Nome"] = "TOTAIS";
                    totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue;
                    totalRow["NomeProduto"] = "";
                    totalRow["Preco"] = 0.0;
                    totalRow["QuantidadeVendida"] = totalQuantidadeVendida;
                    totalRow["QuantidadeDevolvida"] = totalQuantidadeDevolvida;
                    totalRow["ValorRecebido"] = totalValorRecebido;
                    totalRow["Comissao"] = totalComissao;
                    totalRow["DataPrestacao"] = DBNull.Value;
                    totalRow["EntregaID"] = DBNull.Value;
                    totalRow["PrestacaoID"] = DBNull.Value;
                    totalRow["VendedorID"] = DBNull.Value;
                    dt.Rows.Add(totalRow);
                }

                LogUtil.WriteLog($"PesquisaVendasConcluidasPorVendedor concluída com {dt.Rows.Count} linhas para NomeVendedor: {nomeVendedor}");
                return dt;
            }
            catch (Exception erro)
            {
                LogUtil.WriteLog($"Erro em PesquisaVendasConcluidasPorVendedor: {erro.Message}");
                throw erro;
            }
            finally
            {
                conn.Close();
            }
        }
        public void AtualizarPrestacaoConcluida(int prestacaoID, int entregaID, int quantidadeVendida, int quantidadeDevolvida, double valorRecebido, double comissao, DateTime dataPrestacao)
        {
            LogUtil.WriteLog($"Iniciando AtualizarPrestacao para PrestacaoID: {prestacaoID}, EntregaID: {entregaID}");
                                string query = @"
                            UPDATE PrestacaoContas 
                            SET QuantidadeVendida = @QuantidadeVendida,
                                QuantidadeDevolvida = @QuantidadeDevolvida,
                                ValorRecebido = @ValorRecebido,
                                Comissao = @Comissao,
                                DataPrestacao = @DataPrestacao
                            WHERE PrestacaoID = @PrestacaoID AND EntregaID = @EntregaID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para AtualizarPrestacao.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        cmd.Parameters.AddWithValue("@EntregaID", entregaID);
                        cmd.Parameters.AddWithValue("@QuantidadeVendida", quantidadeVendida);
                        cmd.Parameters.AddWithValue("@QuantidadeDevolvida", quantidadeDevolvida);
                        cmd.Parameters.AddWithValue("@ValorRecebido", valorRecebido);
                        cmd.Parameters.AddWithValue("@Comissao", comissao);
                        cmd.Parameters.AddWithValue("@DataPrestacao", dataPrestacao.ToString("yyyy-MM-dd HH:mm:ss"));
                        LogUtil.WriteLog("Executando UPDATE em PrestacaoContas...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Prestação atualizada com sucesso. Linhas afetadas: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em AtualizarPrestacao: {ex.Message}");
                throw;
            }
        }
        public List<PrestacaoContasModel> CarregarPrestacoesPorEntregaID(int entregaID)
        {
            LogUtil.WriteLog($"Iniciando CarregarPrestacoesPorEntregaID com EntregaID: {entregaID}");
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
                            COALESCE(v.Nome, 'Vendedor Excluído') AS Nome, 
                            e.VendedorID
                        FROM PrestacaoContas pc
                        INNER JOIN Entregas e ON pc.EntregaID = e.EntregaID
                        INNER JOIN Vendedores v ON e.VendedorID = v.VendedorID
                        WHERE pc.EntregaID = @EntregaID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para CarregarPrestacoesPorEntregaID.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntregaID", entregaID);
                        LogUtil.WriteLog("Executando query em CarregarPrestacoesPorEntregaID...");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prestacao = new PrestacaoContasModel
                                {
                                    PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                    EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                    QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                    QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                    ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                    Comissao = Convert.ToDouble(reader["Comissao"]),
                                    DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"]),
                                    Nome = reader["Nome"].ToString(),
                                    VendedorID = Convert.ToInt32(reader["VendedorID"])
                                };
                                prestacoes.Add(prestacao);
                                LogUtil.WriteLog($"Prestação carregada: PrestacaoID={prestacao.PrestacaoID}, Nome={prestacao.Nome}");
                            }
                        }
                    }
                }
                LogUtil.WriteLog($"CarregarPrestacoesPorEntregaID concluído com {prestacoes.Count} prestações.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em CarregarPrestacoesPorEntregaID: {ex.Message}");
                throw;
            }
            return prestacoes;
        }
        // Método para excluir contas prestadas - implementado em 04/03/2025
        public void ExcluirPrestacaoPorEntregaID(int entregaID)
        {
            LogUtil.WriteLog($"Iniciando ExcluirPrestacaoPorEntregaID para EntregaID: {entregaID}");
            string query = "DELETE FROM PrestacaoContas WHERE EntregaID = @EntregaID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para ExcluirPrestacaoPorEntregaID.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntregaID", entregaID);
                        LogUtil.WriteLog("Executando DELETE em PrestacaoContas...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Prestação excluída com sucesso. Linhas afetadas: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em ExcluirPrestacaoPorEntregaID: {ex.Message}");
                throw;
            }
        }
        public void ExcluirPrestacaoDeContasConcluidas(int prestacaoID, int entregaID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                Log($"Excluindo prestação de contas {prestacaoID} para entrega {entregaID}");
                string query = "DELETE FROM PrestacaoContas WHERE PrestacaoID = @PrestacaoID AND EntregaID = @EntregaID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                    cmd.Parameters.AddWithValue("@EntregaID", entregaID);
                    cmd.ExecuteNonQuery();
                    Log($"Prestação de contas {prestacaoID} excluída com sucesso.");
                }
            }
        }
        public void AtualizarPrestacaoDeContasConcluidas(PrestacaoContasModel prestacao)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
            UPDATE PrestacaoContas 
            SET QuantidadeVendida = @QuantidadeVendida, QuantidadeDevolvida = @QuantidadeDevolvida, 
                ValorRecebido = @ValorRecebido, Comissao = @Comissao, DataPrestacao = @DataPrestacao
            WHERE PrestacaoID = @PrestacaoID AND EntregaID = @EntregaID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    Log($"Atualizando prestação de contas {prestacao.PrestacaoID} para entrega {prestacao.EntregaID}");
                    cmd.Parameters.AddWithValue("@QuantidadeVendida", prestacao.QuantidadeVendida);
                    cmd.Parameters.AddWithValue("@QuantidadeDevolvida", prestacao.QuantidadeDevolvida);
                    cmd.Parameters.AddWithValue("@ValorRecebido", prestacao.ValorRecebido);
                    cmd.Parameters.AddWithValue("@Comissao", prestacao.Comissao);
                    cmd.Parameters.AddWithValue("@DataPrestacao", prestacao.DataPrestacao);
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacao.PrestacaoID);
                    cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
                    cmd.ExecuteNonQuery();
                    Log($"Prestação de contas {prestacao.PrestacaoID} atualizada com sucesso.");
                }
            }
        }
        public List<PrestacaoContasModel> CarregarPrestacoesPorID(int prestacaoID)
        {
            LogUtil.WriteLog($"Iniciando CarregarPrestacoesPorID com PrestacaoID: {prestacaoID}");
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
            COALESCE(v.Nome, 'Vendedor Excluído') AS Nome, 
            e.VendedorID  -- Usando Entregas.VendedorID
        FROM PrestacaoContas pc
        LEFT JOIN Entregas e ON pc.EntregaID = e.EntregaID
        LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
        WHERE pc.PrestacaoID = @PrestacaoID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para CarregarPrestacoesPorID.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        LogUtil.WriteLog("Executando query em CarregarPrestacoesPorID...");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prestacao = new PrestacaoContasModel
                                {
                                    PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                    EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                    QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                    QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                    ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                    Comissao = Convert.ToDouble(reader["Comissao"]),
                                    DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"]),
                                    Nome = reader["Nome"].ToString(),
                                    VendedorID = Convert.ToInt32(reader["VendedorID"])
                                };
                                prestacoes.Add(prestacao);
                                LogUtil.WriteLog($"Prestação carregada: PrestacaoID={prestacao.PrestacaoID}, Nome={prestacao.Nome}");
                            }
                        }
                    }
                }
                LogUtil.WriteLog($"CarregarPrestacoesPorID concluído com {prestacoes.Count} prestações.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em CarregarPrestacoesPorID: {ex.Message}");
                throw;
            }
            return prestacoes;
        }


        //public DataTable listaEntregasConcluidas()
        //{
        //    var conn = Conexao.Conex();
        //    try
        //    {
        //        conn.Open();
        //        string query = @"
        //    SELECT 
        //        COALESCE(v.Nome, 'Vendedor Excluído') AS Nome, -- 1
        //        COALESCE(e.QuantidadeEntregue, 0) AS QuantidadeEntregue, -- 2
        //        p.NomeProduto,                                -- 3
        //        COALESCE(p.Preco, 0) AS Preco,                -- 4
        //        COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida, -- 5
        //        COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida, -- 6
        //        COALESCE(pc.ValorRecebido, 0) AS ValorRecebido, -- 7
        //        COALESCE(pc.Comissao, 0) AS Comissao,         -- 8
        //        pc.DataPrestacao,                             -- 9
        //        pc.EntregaID,                                 -- 10 (adicionado)
        //        pc.PrestacaoID                                -- 11 (adicionado)
        //    FROM PrestacaoContas pc
        //    LEFT JOIN Entregas e ON pc.EntregaID = e.EntregaID
        //    LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
        //    LEFT JOIN Produtos p ON e.ProdutoID = p.ProdutoID";

        //        SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
        //        SQLiteDataAdapter da = new SQLiteDataAdapter();
        //        da.SelectCommand = sqlcomando;
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        // Adicionar linha de totais
        //        if (dt.Rows.Count > 0)
        //        {
        //            long totalQuantidadeEntregue = dt.AsEnumerable()
        //                .Sum(row => row["QuantidadeEntregue"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeEntregue"]));
        //            long totalQuantidadeVendida = dt.AsEnumerable()
        //                .Sum(row => row["QuantidadeVendida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeVendida"]));
        //            long totalQuantidadeDevolvida = dt.AsEnumerable()
        //                .Sum(row => row["QuantidadeDevolvida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeDevolvida"]));
        //            double totalValorRecebido = dt.AsEnumerable()
        //                .Sum(row => row["ValorRecebido"] is DBNull ? 0.0 : Convert.ToDouble(row["ValorRecebido"]));
        //            double totalComissao = dt.AsEnumerable()
        //                .Sum(row => row["Comissao"] is DBNull ? 0.0 : Convert.ToDouble(row["Comissao"]));

        //            DataRow totalRow = dt.NewRow();
        //            totalRow["Nome"] = "TOTAIS";           // 1
        //            totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue; // 2
        //            totalRow["NomeProduto"] = "";                  // 3 (vazio para totais)
        //            totalRow["Preco"] = 0.0;                       // 4 (usar 0 em vez de DBNull)
        //            totalRow["QuantidadeVendida"] = totalQuantidadeVendida;   // 5
        //            totalRow["QuantidadeDevolvida"] = totalQuantidadeDevolvida; // 6
        //            totalRow["ValorRecebido"] = totalValorRecebido;           // 7
        //            totalRow["Comissao"] = totalComissao;                     // 8
        //            totalRow["DataPrestacao"] = DBNull.Value;                 // 9 (mantém DBNull)
        //            totalRow["EntregaID"] = DBNull.Value;                     // 10 (adicionado)
        //            totalRow["PrestacaoID"] = DBNull.Value;                   // 11 (adicionado)
        //            dt.Rows.Add(totalRow);

        //            // Depuração para confirmar a linha de totais
        //            Console.WriteLine("Linha de totais adicionada: " + totalRow["Nome"]);
        //        }

        //        return dt;
        //    }
        //    catch (Exception erro)
        //    {
        //        throw erro;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}
        public List<PrestacaoContasModel> CarregarPrestacoesPorVendedorID(int vendedorID)
        {
            LogUtil.WriteLog($"Iniciando CarregarPrestacoesPorVendedorID com VendedorID: {vendedorID}");
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
            COALESCE(v.Nome, 'Vendedor Excluído') AS Nome, 
            e.VendedorID
        FROM PrestacaoContas pc
        INNER JOIN Entregas e ON pc.EntregaID = e.EntregaID
        INNER JOIN Vendedores v ON e.VendedorID = v.VendedorID
        WHERE e.VendedorID = @VendedorID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para CarregarPrestacoesPorVendedorID.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VendedorID", vendedorID);
                        LogUtil.WriteLog("Executando query em CarregarPrestacoesPorVendedorID...");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prestacao = new PrestacaoContasModel
                                {
                                    PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                    EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                    QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                    QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                    ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                    Comissao = Convert.ToDouble(reader["Comissao"]),
                                    DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"]),
                                    Nome = reader["Nome"].ToString(),
                                    VendedorID = Convert.ToInt32(reader["VendedorID"])
                                };
                                prestacoes.Add(prestacao);
                                LogUtil.WriteLog($"Prestação carregada: PrestacaoID={prestacao.PrestacaoID}, Nome={prestacao.Nome}");
                            }
                        }
                    }
                }
                LogUtil.WriteLog($"CarregarPrestacoesPorVendedorID concluído com {prestacoes.Count} prestações.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em CarregarPrestacoesPorVendedorID: {ex.Message}");
                throw;
            }
            return prestacoes;
        }
        public DataTable listaEntregasConcluidas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();
                string query = @"
            SELECT 
                COALESCE(v.Nome, 'Vendedor Excluído') AS Nome,
                COALESCE(e.QuantidadeEntregue, 0) AS QuantidadeEntregue,
                p.NomeProduto,
                COALESCE(p.Preco, 0) AS Preco,
                COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida,
                COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida,
                COALESCE(pc.ValorRecebido, 0) AS ValorRecebido,
                COALESCE(pc.Comissao, 0) AS Comissao,
                pc.DataPrestacao,
                pc.EntregaID,
                pc.PrestacaoID,
                e.VendedorID  -- Adicionar VendedorID
            FROM PrestacaoContas pc
            LEFT JOIN Entregas e ON pc.EntregaID = e.EntregaID
            LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
            LEFT JOIN Produtos p ON e.ProdutoID = p.ProdutoID";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
                SQLiteDataAdapter da = new SQLiteDataAdapter();
                da.SelectCommand = sqlcomando;
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    long totalQuantidadeEntregue = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeEntregue"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeEntregue"]));
                    long totalQuantidadeVendida = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeVendida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeVendida"]));
                    long totalQuantidadeDevolvida = dt.AsEnumerable()
                        .Sum(row => row["QuantidadeDevolvida"] is DBNull ? 0 : Convert.ToInt64(row["QuantidadeDevolvida"]));
                    double totalValorRecebido = dt.AsEnumerable()
                        .Sum(row => row["ValorRecebido"] is DBNull ? 0.0 : Convert.ToDouble(row["ValorRecebido"]));
                    double totalComissao = dt.AsEnumerable()
                        .Sum(row => row["Comissao"] is DBNull ? 0.0 : Convert.ToDouble(row["Comissao"]));

                    DataRow totalRow = dt.NewRow();
                    totalRow["Nome"] = "TOTAIS";
                    totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue;
                    totalRow["NomeProduto"] = "";
                    totalRow["Preco"] = 0.0;
                    totalRow["QuantidadeVendida"] = totalQuantidadeVendida;
                    totalRow["QuantidadeDevolvida"] = totalQuantidadeDevolvida;
                    totalRow["ValorRecebido"] = totalValorRecebido;
                    totalRow["Comissao"] = totalComissao;
                    totalRow["DataPrestacao"] = DBNull.Value;
                    totalRow["EntregaID"] = DBNull.Value;
                    totalRow["PrestacaoID"] = DBNull.Value;
                    totalRow["VendedorID"] = DBNull.Value; //--Adicionado
                    dt.Rows.Add(totalRow);
                }

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

        public void SalvarPrestacaoDeContas(PrestacaoContasModel prestacao)
        {
            LogUtil.WriteLog($"Iniciando SalvarPrestacaoDeContas para EntregaID: {prestacao.EntregaID}");
            string query = @"
            INSERT INTO PrestacaoContas (EntregaID, QuantidadeVendida, QuantidadeDevolvida, ValorRecebido, Comissao, DataPrestacao)
            VALUES (@EntregaID, @QuantidadeVendida, @QuantidadeDevolvida, @ValorRecebido, @Comissao, @DataPrestacao);
            SELECT last_insert_rowid();"; // Retorna o ID gerado

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para SalvarPrestacaoDeContas.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
                        cmd.Parameters.AddWithValue("@QuantidadeVendida", prestacao.QuantidadeVendida);
                        cmd.Parameters.AddWithValue("@QuantidadeDevolvida", prestacao.QuantidadeDevolvida);
                        cmd.Parameters.AddWithValue("@ValorRecebido", prestacao.ValorRecebido);
                        cmd.Parameters.AddWithValue("@Comissao", prestacao.Comissao);
                        cmd.Parameters.AddWithValue("@DataPrestacao", prestacao.DataPrestacao.ToString("yyyy-MM-dd HH:mm:ss"));

                        // Executa e obtém o PrestacaoID gerado
                        prestacao.PrestacaoID = Convert.ToInt32(cmd.ExecuteScalar());
                        LogUtil.WriteLog($"Prestação salva com PrestacaoID: {prestacao.PrestacaoID}");
                    }

                    // Marcar a entrega como realizada
                    string updateEntregaQuery = "UPDATE Entregas SET PrestacaoRealizada = 1 WHERE EntregaID = @EntregaID";
                    using (var cmd = new SQLiteCommand(updateEntregaQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em SalvarPrestacaoDeContas: {ex.Message}");
                throw;
            }
        }

        //public void SalvarPrestacaoDeContas(PrestacaoContasModel prestacao)
        //{
        //    using (var conn = Conexao.Conex()) // Assumindo que Conexao.Conex() retorna uma SQLiteConnection
        //    {
        //        conn.Open();

        //        // Inserir a prestação de contas
        //        string insertQuery = @"INSERT INTO PrestacaoContas (EntregaID, QuantidadeVendida, QuantidadeDevolvida, ValorRecebido, Comissao, DataPrestacao) 
        //                           VALUES (@EntregaID, @QuantidadeVendida, @QuantidadeDevolvida, @ValorRecebido, @Comissao, @DataPrestacao)";
        //        using (var cmd = new SQLiteCommand(insertQuery, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
        //            cmd.Parameters.AddWithValue("@QuantidadeVendida", prestacao.QuantidadeVendida);
        //            cmd.Parameters.AddWithValue("@QuantidadeDevolvida", prestacao.QuantidadeDevolvida);
        //            cmd.Parameters.AddWithValue("@ValorRecebido", prestacao.ValorRecebido);
        //            cmd.Parameters.AddWithValue("@Comissao", prestacao.Comissao);
        //            cmd.Parameters.AddWithValue("@DataPrestacao", prestacao.DataPrestacao);
        //            cmd.ExecuteNonQuery();
        //        }

        //        // Atualizar o campo PrestacaoRealizada na tabela Entregas
        //        string updateQuery = @"UPDATE Entregas 
        //                          SET PrestacaoRealizada = 1 
        //                          WHERE EntregaID = @EntregaID";
        //        using (var cmd = new SQLiteCommand(updateQuery, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
        public void AlterarPrestacao(PrestacaoContasModel prestacao)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "UPDATE PrestacaoContas SET EntregaID = @EntregaID, QuantidadeVendida = @QuantidadeVendida, QuantidadeDevolvida = @QuantidadeDevolvida, ValorRecebido = @ValorRecebido, Comissao = @Comissao, DataPrestacao = @DataPrestacao WHERE PrestacaoID = @PrestacaoID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacao.PrestacaoID);
                    cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
                    cmd.Parameters.AddWithValue("@QuantidadeVendida", prestacao.QuantidadeVendida);
                    cmd.Parameters.AddWithValue("@QuantidadeDevolvida", prestacao.QuantidadeDevolvida);
                    cmd.Parameters.AddWithValue("@ValorRecebido", prestacao.ValorRecebido);
                    cmd.Parameters.AddWithValue("@Comissao", prestacao.Comissao);
                    cmd.Parameters.AddWithValue("@DataPrestacao", prestacao.DataPrestacao);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void ExcluirPrestacao(Model.PrestacaoContasModel prestacao)
        {
            using (var conexao = Conexao.Conex())
            {
                conexao.Open();
                string sql = "DELETE FROM PrestacaoContas WHERE PrestacaoID = @PrestacaoID";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacao.PrestacaoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // Método para excluir contas órfãs
        public int ExcluirContasOrfas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();

                string query = @"
                DELETE FROM PrestacaoContas 
                WHERE EntregaID NOT IN (SELECT EntregaID FROM Entregas);";

                using (SQLiteCommand sqlcomando = new SQLiteCommand(query, conn))
                {
                    int rowsAffected = sqlcomando.ExecuteNonQuery();
                    return rowsAffected; // Retorna o número de contas excluídas
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao excluir contas órfãs: " + erro.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public PrestacaoContasModel PesquisarPorCodigoPrestacao(string pesquisa)
        {
            PrestacaoContasModel prestacao = null;

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT * FROM PrestacaoContas WHERE PrestacaoID = @Pesquisa";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            prestacao = new PrestacaoContasModel
                            {
                                PrestacaoID = Convert.ToInt32(reader["PrestacaoID"]),
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                QuantidadeVendida = Convert.ToInt32(reader["QuantidadeVendida"]),
                                QuantidadeDevolvida = Convert.ToInt32(reader["QuantidadeDevolvida"]),
                                ValorRecebido = Convert.ToDouble(reader["ValorRecebido"]),
                                Comissao = Convert.ToDouble(reader["Comissao"]),
                                DataPrestacao = Convert.ToDateTime(reader["DataPrestacao"])
                            };
                        }
                    }
                }
            }

            return prestacao;
        }

        // Método para obter o ID da prestação de contas por entrega implementado em 04/03/2025
        public int ObterPrestacaoIDPorEntrega(string entregaID)
        {
            int prestacaoID = 0;
            try
            {
                string query = @"
            SELECT PrestacaoID 
            FROM PrestacaoContas 
            WHERE EntregaID = @EntregaID 
            LIMIT 1";

                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntregaID", entregaID);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            prestacaoID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter PrestacaoID para EntregaID {entregaID}: {ex.Message}");
            }
            return prestacaoID;
        }

    }
}
