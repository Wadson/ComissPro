using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;
using static ComissPro.Utilitario;

namespace ComissPro
{
    internal class FluxoCaixaDAL
    {

        public void ExcluirMovimentacoesPorPrestacao(int prestacaoID)
        {
            LogUtil.WriteLog($"Iniciando ExcluirMovimentacoesPorPrestacao para PrestacaoID: {prestacaoID}");
            string query = "DELETE FROM FluxoCaixa WHERE PrestacaoID = @PrestacaoID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para ExcluirMovimentacoesPorPrestacao.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        LogUtil.WriteLog("Executando DELETE em FluxoCaixa...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Movimentações excluídas. Linhas afetadas: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em ExcluirMovimentacoesPorPrestacao: {ex.Message}");
                throw;
            }
        }
        public void AtualizarMovimentacoesPorPrestacao(int prestacaoID, double valorRecebido, double comissao)
        {
            LogUtil.WriteLog($"Iniciando AtualizarMovimentacoesPorPrestacao para PrestacaoID: {prestacaoID}");
            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para AtualizarMovimentacoesPorPrestacao.");

                    // Atualizar a entrada (ValorRecebido)
                    string updateEntradaQuery = @"
                UPDATE FluxoCaixa 
                SET Valor = @Valor, DataMovimentacao = @DataMovimentacao
                WHERE PrestacaoID = @PrestacaoID AND TipoMovimentacao = 'ENTRADA'";
                    using (var cmd = new SQLiteCommand(updateEntradaQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        cmd.Parameters.AddWithValue("@Valor", valorRecebido);
                        cmd.Parameters.AddWithValue("@DataMovimentacao", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        LogUtil.WriteLog("Executando UPDATE para entrada em FluxoCaixa...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Entrada atualizada. Linhas afetadas: {rowsAffected}");

                        // Se não houver entrada existente, inserir uma nova
                        if (rowsAffected == 0)
                        {
                            string insertEntradaQuery = @"
                        INSERT INTO FluxoCaixa (TipoMovimentacao, Valor, DataMovimentacao, Descricao, PrestacaoID)
                        VALUES ('ENTRADA', @Valor, @DataMovimentacao, 'Valor recebido de prestação', @PrestacaoID)";
                            cmd.CommandText = insertEntradaQuery;
                            LogUtil.WriteLog("Executando INSERT para nova entrada em FluxoCaixa...");
                            cmd.ExecuteNonQuery();
                            LogUtil.WriteLog("Nova entrada inserida com sucesso.");
                        }
                    }

                    // Atualizar a saída (Comissao)
                    string updateSaidaQuery = @"
                UPDATE FluxoCaixa 
                SET Valor = @Valor, DataMovimentacao = @DataMovimentacao
                WHERE PrestacaoID = @PrestacaoID AND TipoMovimentacao = 'SAÍDA'";
                    using (var cmd = new SQLiteCommand(updateSaidaQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        cmd.Parameters.AddWithValue("@Valor", comissao);
                        cmd.Parameters.AddWithValue("@DataMovimentacao", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        LogUtil.WriteLog("Executando UPDATE para saída em FluxoCaixa...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Saída atualizada. Linhas afetadas: {rowsAffected}");

                        // Se não houver saída existente, inserir uma nova
                        if (rowsAffected == 0)
                        {
                            string insertSaidaQuery = @"
                        INSERT INTO FluxoCaixa (TipoMovimentacao, Valor, DataMovimentacao, Descricao, PrestacaoID)
                        VALUES ('SAÍDA', @Valor, @DataMovimentacao, 'Comissão paga por prestação', @PrestacaoID)";
                            cmd.CommandText = insertSaidaQuery;
                            LogUtil.WriteLog("Executando INSERT para nova saída em FluxoCaixa...");
                            cmd.ExecuteNonQuery();
                            LogUtil.WriteLog("Nova saída inserida com sucesso.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em AtualizarMovimentacoesPorPrestacao: {ex.Message}");
                throw;
            }
        }

       
        public void ExcluirMovimentacoesPorPrestacoes(int prestacaoID) // ùltima alteração em 04/03/2025 nova versão
        {
            LogUtil.WriteLog($"Iniciando ExcluirMovimentacoesPorPrestacao para PrestacaoID: {prestacaoID}");
            string query = "DELETE FROM FluxoCaixa WHERE PrestacaoID = @PrestacaoID";

            try
            {
                using (var conn = Conexao.Conex())
                {
                    conn.Open();
                    LogUtil.WriteLog("Conexão aberta para ExcluirMovimentacoesPorPrestacao.");
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                        LogUtil.WriteLog("Executando DELETE em FluxoCaixa...");
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogUtil.WriteLog($"Movimentações excluídas. Linhas afetadas: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em ExcluirMovimentacoesPorPrestacao: {ex.Message}");
                throw;
            }
        }
        public void AtualizarMovimentacao(PrestacaoContasModel prestacao)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string queryEntrada = "UPDATE FluxoCaixa SET Valor = @Valor WHERE PrestacaoID = @PrestacaoID AND TipoMovimentacao = 'ENTRADA'";
                string querySaida = "UPDATE FluxoCaixa SET Valor = @Valor WHERE PrestacaoID = @PrestacaoID AND TipoMovimentacao = 'SAIDA'";

                using (var cmd = new SQLiteCommand(queryEntrada, conn))
                {
                    cmd.Parameters.AddWithValue("@Valor", prestacao.ValorRecebido);
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacao.PrestacaoID);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SQLiteCommand(querySaida, conn))
                {
                    cmd.Parameters.AddWithValue("@Valor", prestacao.Comissao);
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacao.PrestacaoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void RegistrarMovimentacao(FluxoCaixaModel movimentacao)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                    INSERT INTO FluxoCaixa (TipoMovimentacao, Valor, DataMovimentacao, Descricao, PrestacaoID)
                    VALUES (@TipoMovimentacao, @Valor, @DataMovimentacao, @Descricao, @PrestacaoID)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TipoMovimentacao", movimentacao.TipoMovimentacao);
                    cmd.Parameters.AddWithValue("@Valor", movimentacao.Valor);
                    cmd.Parameters.AddWithValue("@DataMovimentacao", movimentacao.DataMovimentacao);
                    cmd.Parameters.AddWithValue("@Descricao", movimentacao.Descricao ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrestacaoID", movimentacao.PrestacaoID.HasValue ? movimentacao.PrestacaoID.Value : (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        

        public void FecharCaixaDiario(DateTime data) // Adicionar parâmetro de data
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "UPDATE FluxoCaixa SET Fechado = 1 WHERE DATE(DataMovimentacao) = @DataSelecionada AND Fechado = 0";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataSelecionada", data.ToString("yyyy-MM-dd"));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    LogUtil.WriteLog($"Caixa fechado para {data.ToString("yyyy-MM-dd")}. Linhas marcadas como fechadas: {rowsAffected}");
                }
            }
        }

        public bool IsCaixaFechado(DateTime data) // Novo método para verificar se o caixa está fechado
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM FluxoCaixa WHERE DATE(DataMovimentacao) = @DataSelecionada AND Fechado = 0";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataSelecionada", data.ToString("yyyy-MM-dd"));
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    LogUtil.WriteLog($"Verificando caixa para {data.ToString("yyyy-MM-dd")}: {count} registros abertos encontrados.");
                    return count == 0; // Retorna true se não houver registros abertos (caixa fechado)
                }
            }
        }

        // Atualizar métodos existentes para consistência
        public List<FluxoCaixaModel> ObterMovimentacoesPorData(DateTime data)
        {
            var movimentacoes = new List<FluxoCaixaModel>();
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                SELECT * FROM FluxoCaixa 
                WHERE DATE(DataMovimentacao) = @DataSelecionada AND Fechado = 0
                ORDER BY DataMovimentacao DESC";
                LogUtil.WriteLog($"Executando ObterMovimentacoesPorData para data: {data.ToString("yyyy-MM-dd")}");
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataSelecionada", data.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var mov = new FluxoCaixaModel
                            {
                                FluxoCaixaID = Convert.ToInt32(reader["FluxoCaixaID"]),
                                TipoMovimentacao = reader["TipoMovimentacao"].ToString(),
                                Valor = Convert.ToDouble(reader["Valor"]),
                                DataMovimentacao = Convert.ToDateTime(reader["DataMovimentacao"]),
                                Descricao = reader["Descricao"] == DBNull.Value ? null : reader["Descricao"].ToString(),
                                PrestacaoID = reader["PrestacaoID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PrestacaoID"])
                            };
                            movimentacoes.Add(mov);
                            LogUtil.WriteLog($"Movimentação encontrada: ID={mov.FluxoCaixaID}, Tipo={mov.TipoMovimentacao}, Data={mov.DataMovimentacao}");
                        }
                    }
                }
                LogUtil.WriteLog($"Total de movimentações encontradas: {movimentacoes.Count}");
            }
            return movimentacoes;
        }

        public double CalcularSaldoPorData(DateTime data)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                SELECT 
                    (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'ENTRADA' AND DATE(DataMovimentacao) = @DataSelecionada AND Fechado = 0) -
                    (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'SAÍDA' AND DATE(DataMovimentacao) = @DataSelecionada AND Fechado = 0) AS Saldo";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataSelecionada", data.ToString("yyyy-MM-dd"));
                    var result = cmd.ExecuteScalar();
                    double saldo = result == DBNull.Value ? 0 : Convert.ToDouble(result);
                    LogUtil.WriteLog($"Saldo calculado para data {data.ToString("yyyy-MM-dd")}: {saldo}");
                    return saldo;
                }
            }
        }

        public double CalcularSaldoDoDia()
        {
            return CalcularSaldoPorData(DateTime.Today);
        }

    }
}
