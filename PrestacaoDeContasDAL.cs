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
                string query = @"
         SELECT 
             COALESCE(v.Nome, 'Vendedor Excluído') AS NomeVendedor, -- 1
             COALESCE(e.QuantidadeEntregue, 0) AS QuantidadeEntregue, -- 2
             p.NomeProduto,                                -- 3
             COALESCE(p.Preco, 0) AS Preco,                -- 4
             COALESCE(pc.QuantidadeVendida, 0) AS QuantidadeVendida, -- 5
             COALESCE(pc.QuantidadeDevolvida, 0) AS QuantidadeDevolvida, -- 6
             COALESCE(pc.ValorRecebido, 0) AS ValorRecebido, -- 7
             COALESCE(pc.Comissao, 0) AS Comissao,         -- 8
             pc.DataPrestacao                              -- 9
         FROM PrestacaoContas pc
         LEFT JOIN Entregas e ON pc.EntregaID = e.EntregaID
         LEFT JOIN Vendedores v ON e.VendedorID = v.VendedorID
         LEFT JOIN Produtos p ON e.ProdutoID = p.ProdutoID";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
                SQLiteDataAdapter da = new SQLiteDataAdapter();
                da.SelectCommand = sqlcomando;
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Adicionar linha de totais
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
                    totalRow["NomeVendedor"] = "TOTAIS";           // 1
                    totalRow["QuantidadeEntregue"] = totalQuantidadeEntregue; // 2
                    totalRow["NomeProduto"] = "";                  // 3 (vazio para totais)
                    totalRow["Preco"] = 0.0;                       // 4 (usar 0 em vez de DBNull)
                    totalRow["QuantidadeVendida"] = totalQuantidadeVendida;   // 5
                    totalRow["QuantidadeDevolvida"] = totalQuantidadeDevolvida; // 6
                    totalRow["ValorRecebido"] = totalValorRecebido;           // 7
                    totalRow["Comissao"] = totalComissao;                     // 8
                    totalRow["DataPrestacao"] = DBNull.Value;                 // 9 (mantém DBNull)
                    dt.Rows.Add(totalRow);

                    // Depuração para confirmar a linha de totais
                    Console.WriteLine("Linha de totais adicionada: " + totalRow["NomeVendedor"]);
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

    }
}
