using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;

namespace ComissPro
{
    internal class FluxoCaixaDAL
    {       
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

        public List<FluxoCaixaModel> ObterMovimentacoesDoDia()
        {
            var movimentacoes = new List<FluxoCaixaModel>();
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                    SELECT * FROM FluxoCaixa 
                    WHERE DATE(DataMovimentacao) = DATE('now') 
                    ORDER BY DataMovimentacao DESC";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            movimentacoes.Add(new FluxoCaixaModel
                            {
                                FluxoCaixaID = Convert.ToInt32(reader["FluxoCaixaID"]),
                                TipoMovimentacao = reader["TipoMovimentacao"].ToString(),
                                Valor = Convert.ToDouble(reader["Valor"]),
                                DataMovimentacao = Convert.ToDateTime(reader["DataMovimentacao"]),
                                Descricao = reader["Descricao"] == DBNull.Value ? null : reader["Descricao"].ToString(),
                                PrestacaoID = reader["PrestacaoID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PrestacaoID"])
                            });
                        }
                    }
                }
            }
            return movimentacoes;
        }

        public double CalcularSaldoDoDia()
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
                    SELECT 
                        (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'ENTRADA' AND DATE(DataMovimentacao) = DATE('now')) -
                        (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'SAIDA' AND DATE(DataMovimentacao) = DATE('now')) AS Saldo";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDouble(result);
                }
            }
        }

        public void FecharCaixaDiario()
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                // Aqui você pode optar por arquivar os dados ou apenas limpá-los
                string query = "DELETE FROM FluxoCaixa WHERE DATE(DataMovimentacao) = DATE('now')";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
