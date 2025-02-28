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
        public DataTable listaEntregas()
        {
            var conn = Conexao.Conex();
            try
            {
                conn.Open();

                string query = @"SELECT
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
                            Produtos ON Entregas.ProdutoID = Produtos.ProdutoID;
";

                SQLiteCommand sqlcomando = new SQLiteCommand(query, conn);
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
        public void SalvarEntregas(EntregasModel entrega)
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
        public EntregasModel PesquisarPorCodigoEntrega(string pesquisa)
        {
            EntregasModel entrega = null;

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT * FROM Entregas WHERE EntregaID = @Pesquisa";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entrega = new EntregasModel
                            {
                                EntregaID = Convert.ToInt32(reader["EntregaID"]),
                                VendedorID = Convert.ToInt32(reader["VendedorID"]),
                                ProdutoID = Convert.ToInt32(reader["ProdutoID"]),
                                QuantidadeEntregue = Convert.ToInt32(reader["QuantidadeEntregue"]),
                                DataEntrega = Convert.ToDateTime(reader["DataEntrega"])
                            };
                        }
                    }
                }
            }

            return entrega;
        }

    }
}
