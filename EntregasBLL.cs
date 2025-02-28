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
    internal class EntregasBLL
    {
        EntregasDal entregasDAL = null;

        public DataTable Listar()
        {
            DataTable dtable = new DataTable();
            try
            {
                entregasDAL = new EntregasDal();
                dtable = entregasDAL.listaEntregas();
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return dtable;
        }

        public void Salvar(Model.EntregasModel entregas)
        {
            try
            {
                entregasDAL = new EntregasDal();
                entregasDAL.InserirEntrega(entregas);
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
        public void Excluir(Model.EntregasModel entrega)
        {
            try
            {
                Log($"Iniciando exclusão do Entregas com Código: {entrega.EntregaID}");
                entregasDAL = new EntregasDal();
                entregasDAL.Excluir(entrega);
                Log("Entrega excluída com sucesso.");
            }
            catch (Exception erro)
            {
                Log($"Erro ao limpar o formulário: {erro.Message}");
            }
        }

        public void Alterar(Model.EntregasModel entregas)
        {
            try
            {
                entregasDAL = new EntregasDal();
                entregasDAL.AlterarEntrega(entregas);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public Model.EntregasModel pesquisar(string pesquisa)
        {
            var conn = Conexao.Conex();

            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT * FROM Entregas WHERE VendedorID LIKE '" + pesquisa + "%'", conn);
                conn.Open();
                SQLiteDataReader datareader;
                Model.EntregasModel objeto = new Model.EntregasModel();
                datareader = sql.ExecuteReader(CommandBehavior.CloseConnection);
                while (datareader.Read())
                {
                    if (datareader.IsDBNull(0))
                    {
                        string erro;
                        erro = "Nenhum registro encontrado";
                        Console.Write(erro);
                    }
                    else
                    {
                        objeto.EntregaID = Convert.ToInt32(datareader["EntregaID"]);
                        objeto.VendedorID = Convert.ToInt32(datareader["VendedorID"]);
                        objeto.ProdutoID = Convert.ToInt32(datareader["ProdutoID"]);
                        objeto.VendedorID = Convert.ToInt32(datareader["QuantidadeEntregue"]);
                        objeto.VendedorID = Convert.ToInt32(datareader["DataEntrega"]);
                    }
                }
                return objeto;
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
    }
}
