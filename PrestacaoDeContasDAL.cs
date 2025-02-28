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
    internal class PrestacaoDeContasDAL
    {
        public DataTable listaEntregas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();
                SQLiteCommand sqlcomando = new SQLiteCommand("SELECT * FROM PrestacaoContas", conn);
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
        public void SalvarPrestacaoDeContas(PrestacaoContasModel prestacao)
        {
            using (var conn = Conexao.Conex()) // Assumindo que Conexao.Conex() retorna uma SQLiteConnection
            {
                conn.Open();

                // Inserir a prestação de contas
                string insertQuery = @"INSERT INTO PrestacaoContas (EntregaID, QuantidadeVendida, QuantidadeDevolvida, ValorRecebido, Comissao, DataPrestacao) 
                                   VALUES (@EntregaID, @QuantidadeVendida, @QuantidadeDevolvida, @ValorRecebido, @Comissao, @DataPrestacao)";
                using (var cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
                    cmd.Parameters.AddWithValue("@QuantidadeVendida", prestacao.QuantidadeVendida);
                    cmd.Parameters.AddWithValue("@QuantidadeDevolvida", prestacao.QuantidadeDevolvida);
                    cmd.Parameters.AddWithValue("@ValorRecebido", prestacao.ValorRecebido);
                    cmd.Parameters.AddWithValue("@Comissao", prestacao.Comissao);
                    cmd.Parameters.AddWithValue("@DataPrestacao", prestacao.DataPrestacao);
                    cmd.ExecuteNonQuery();
                }

                // Atualizar o campo PrestacaoRealizada na tabela Entregas
                string updateQuery = @"UPDATE Entregas 
                                  SET PrestacaoRealizada = 1 
                                  WHERE EntregaID = @EntregaID";
                using (var cmd = new SQLiteCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EntregaID", prestacao.EntregaID);
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
