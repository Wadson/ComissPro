using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;

namespace ComissPro
{
    internal class PrestacaoContasDal
    {
        public void InserirPrestacao(PrestacaoContasModel prestacao)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "INSERT INTO PrestacaoContas (EntregaID, QuantidadeVendida, QuantidadeDevolvida, ValorRecebido, Comissao, DataPrestacao) VALUES (@EntregaID, @QuantidadeVendida, @QuantidadeDevolvida, @ValorRecebido, @Comissao, @DataPrestacao)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
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

        public void ExcluirPrestacao(int prestacaoID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "DELETE FROM PrestacaoContas WHERE PrestacaoID = @PrestacaoID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PrestacaoID", prestacaoID);
                    cmd.ExecuteNonQuery();
                }
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

    }
}
