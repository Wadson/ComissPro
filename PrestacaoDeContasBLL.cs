using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ComissPro
{
    internal class PrestacaoDeContasBLL
    {
        PrestacaoDeContasDAL prestacaoContasDal = null;

        public DataTable Listar()
        {
            DataTable dtable = new DataTable();
            try
            {
                prestacaoContasDal = new PrestacaoDeContasDAL();
                dtable = prestacaoContasDal.listaEntregas();
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return dtable;
        }

        public void Salvar(Model.PrestacaoContasModel entregas)
        {
            try
            {
                prestacaoContasDal = new PrestacaoDeContasDAL();
                prestacaoContasDal.InserirPrestacao(entregas);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        private void Log(string message)
        {
            File.AppendAllText("logExcluirEntregasDeBilhetes.txt", $"{DateTime.Now}: {message}\n");
        }
        public void Excluir(Model.PrestacaoContasModel entregas)
        {
            try
            {
                Log($"Iniciando exclusão de Entregas de Bilhete com ID: {entregas.PrestacaoID}");
                prestacaoContasDal = new PrestacaoDeContasDAL();
                prestacaoContasDal.ExcluirPrestacao(entregas);
                Log("Entregas de bilhete excluído com sucesso.");
            }
            catch (Exception erro)
            {
                Log($"Erro ao limpar o formulário: {erro.Message}");
            }
        }

        public void Alterar(Model.PrestacaoContasModel prestacao)
        {
            try
            {
                prestacaoContasDal = new PrestacaoDeContasDAL();
                prestacaoContasDal.AlterarPrestacao(prestacao);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public List<Model.PrestacaoContasModel> PrestacaoDeContasPesquisarPorVendedor(string nomeVendedor)
        {
            List<Model.PrestacaoContasModel> listaPrestacoes = new List<Model.PrestacaoContasModel>();

            using (var conn = Conexao.Conex())
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT pc.*
                FROM PrestacaoContas pc
                INNER JOIN Entregas e ON pc.EntregaID = e.EntregaID
                INNER JOIN Vendedor v ON e.VendedorID = v.VendedorID
                WHERE v.Nome LIKE @NomeVendedor
                ORDER BY pc.DataPrestacao DESC"; // Ordena da mais recente para a mais antiga

                    using (SQLiteCommand sql = new SQLiteCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@NomeVendedor", nomeVendedor + "%");

                        using (SQLiteDataReader datareader = sql.ExecuteReader())
                        {
                            while (datareader.Read()) // Enquanto houver registros
                            {
                                Model.PrestacaoContasModel prestacao = new Model.PrestacaoContasModel
                                {
                                    PrestacaoID = datareader.GetInt32(datareader.GetOrdinal("PrestacaoID")),
                                    EntregaID = datareader.GetInt32(datareader.GetOrdinal("EntregaID")),
                                    QuantidadeVendida = datareader.GetInt32(datareader.GetOrdinal("QuantidadeVendida")),
                                    QuantidadeDevolvida = datareader.GetInt32(datareader.GetOrdinal("QuantidadeDevolvida")),
                                    ValorRecebido = datareader.GetDouble(datareader.GetOrdinal("ValorRecebido")),
                                    Comissao = datareader.GetDouble(datareader.GetOrdinal("Comissao")),
                                    DataPrestacao = datareader.GetDateTime(datareader.GetOrdinal("DataPrestacao"))
                                };

                                listaPrestacoes.Add(prestacao);
                            }
                        }
                    }
                }
                catch (Exception erro)
                {
                    throw new Exception("Erro ao pesquisar prestações de contas: " + erro.Message);
                }
            }

            return listaPrestacoes;
        }
    }
}
