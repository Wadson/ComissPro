using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace ComissPro
{
    internal class VendedorBLL
    {
        VendedorDAL vendedorDal = null;

        public DataTable Listar()
        {
            DataTable dtable = new DataTable();
            try
            {
                vendedorDal = new VendedorDAL();
                dtable = vendedorDal.listaVendedor();
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return dtable;
        }

        public void Salvar(Model.VendedorMODEL vendedor)
        {
            try
            {
                vendedorDal = new VendedorDAL();
                vendedorDal.Inserir(vendedor);
                Log($"Salvando Vendedor, : {vendedor.VendedorID}");   
            }
            catch (Exception erro)
            {
                Log($"Erro ao salvar vendedor: {erro.Message}");
                throw erro;
            }
        }

        private void Log(string message)
        {
            File.AppendAllText("logTavelaVendedor.txt", $"{DateTime.Now}: {message}\n");
        }
        public void Excluir(Model.VendedorMODEL vendedor)
        {
            try
            {


                Log($"Iniciando exclusão do Vendedores com ID: {vendedor.VendedorID}");
                vendedorDal = new VendedorDAL();
                vendedorDal.Excluir(vendedor);
                Log("Vendedor excluído com sucesso.");
            }
            catch (Exception erro)
            {
                Log($"Erro ao limpar o formulário: {erro.Message}");
            }
        }

        public void Alterar(Model.VendedorMODEL vendedor)
        {
            try
            {
                vendedorDal = new VendedorDAL();
                vendedorDal.Alterar(vendedor);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public Model.VendedorMODEL pesquisar(string pesquisa)
        {
            var conn = Conexao.Conex();

            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT * FROM Vendedores WHERE Nome LIKE '" + pesquisa + "%'", conn);
                conn.Open();
                SQLiteDataReader datareader;
                Model.VendedorMODEL objeto = new Model.VendedorMODEL();
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
                        objeto.VendedorID = Convert.ToInt32(datareader["VendedorID"]);
                        objeto.Nome = datareader["Nome"].ToString();
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
        public Model.VendedorMODEL PesquisaFornecedorEspecial(string pesquisa)
        {
            var conn = Conexao.Conex();
            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT Nome FROM Vendedores WHERE Nome LIKE '" + pesquisa + "%'", conn);
                conn.Open();
                SQLiteDataReader datareader;
                Model.VendedorMODEL objetovendedor = new Model.VendedorMODEL();
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
                        objetovendedor.VendedorID = Convert.ToInt32(datareader["VendedorID"]);
                    }
                }
                return objetovendedor;
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
        public Model.VendedorMODEL PesquisaFornecedorCod(string pesquisa)
        {
            var conn = Conexao.Conex();

            try
            {
                SQLiteCommand sql = new SQLiteCommand("SELECT Nome FROM Vendedores  WHERE VendedorID LIKE '" + pesquisa + "%' ", conn);//AND Pago = false
                conn.Open();
                SQLiteDataReader datareader;

                Model.VendedorMODEL objetoVendedor = new Model.VendedorMODEL();


                datareader = sql.ExecuteReader(CommandBehavior.CloseConnection);
                while (datareader.Read())
                {
                    if (datareader.IsDBNull(0))
                    {
                        string erros = "Nenhum registro ENCONTRADO";
                        Console.WriteLine(erros);
                    }
                    else
                        objetoVendedor.Nome = datareader["Nome"].ToString();
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
