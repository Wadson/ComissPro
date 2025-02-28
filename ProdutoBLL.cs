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
    internal class ProdutoBLL
    {
        ProdutoDAL produtoDal = null;

        public DataTable Listar()
        {
            DataTable dtable = new DataTable();
            try
            {
                produtoDal = new ProdutoDAL();
                dtable = produtoDal.listarProduto();
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return dtable;
        }

        public void Salvar(Model.ProdutoMODEL objetomodel)
        {
            try
            {
                produtoDal = new ProdutoDAL();
                produtoDal.Inserir(objetomodel);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        private void Log(string message)
        {
            File.AppendAllText("logExcluirVendedor.txt", $"{DateTime.Now}: {message}\n");
        }
        public void Excluir(Model.ProdutoMODEL objetomodel)
        {
            try
            {
                Log($"Iniciando exclusão do Vendedores com ID: {objetomodel.ProdutoID}");
                produtoDal = new ProdutoDAL();
                produtoDal.Excluir(objetomodel);
                Log("Vendedor excluído com sucesso.");
            }
            catch (Exception erro)
            {
                Log($"Erro ao limpar o formulário: {erro.Message}");
            }
        }

        public void Alterar(Model.ProdutoMODEL objetoModel)
        {
            try
            {
                produtoDal = new ProdutoDAL();
                produtoDal.Alterar(objetoModel);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public Model.ProdutoMODEL pesquisar(string pesquisa)
        {
            var conn = Conexao.Conex();

            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT * FROM Produtos WHERE Nome LIKE '" + pesquisa + "%'", conn);
                conn.Open();
                SQLiteDataReader datareader;
                Model.ProdutoMODEL objeto = new Model.ProdutoMODEL();
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
                        objeto.ProdutoID = Convert.ToInt32(datareader["ProdutoID"]);
                        objeto.NomeProduto = datareader["NomeProduto"].ToString();
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
       
        public Model.ProdutoMODEL PesquisaProdutoCodigo(string pesquisa)
        {
            var conn = Conexao.Conex();

            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT * FROM Produtos  WHERE ProdutoID LIKE '" + pesquisa + "%' ", conn);//AND Pago = false
                conn.Open();
                SQLiteDataReader datareader;

                Model.ProdutoMODEL objetoVendedor = new Model.ProdutoMODEL();


                datareader = sql.ExecuteReader(CommandBehavior.CloseConnection);
                while (datareader.Read())
                {
                    if (datareader.IsDBNull(0))
                    {
                        string erros = "Nenhum registro ENCONTRADO";
                        Console.WriteLine(erros);
                    }
                    else
                        objetoVendedor.NomeProduto = datareader["NomeProduto"].ToString();
                }
                return objetoVendedor;
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
