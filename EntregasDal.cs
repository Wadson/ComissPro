using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComissPro.Model;

namespace ComissPro
{
    internal class EntregasDal
    {
        public void InserirEntrega(EntregasModel entrega)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "INSERT INTO Entregas (VendedorID, ProdutoID, QuantidadeEntregue, DataEntrega) VALUES (@VendedorID, @ProdutoID, @QuantidadeEntregue, @DataEntrega)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", entrega.VendedorID);
                    cmd.Parameters.AddWithValue("@ProdutoID", entrega.ProdutoID);
                    cmd.Parameters.AddWithValue("@QuantidadeEntregue", entrega.QuantidadeEntregue);
                    cmd.Parameters.AddWithValue("@DataEntrega", entrega.DataEntrega);
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

        public void ExcluirEntrega(int entregaID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "DELETE FROM Entregas WHERE EntregaID = @EntregaID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EntregaID", entregaID);
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
