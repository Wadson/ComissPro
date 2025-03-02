using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ComissPro
{
    internal class VendedorBLL
    {
        private VendedorDAL vendedorDal = new VendedorDAL();

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
            vendedorDal.Salvar(vendedor);
        }

        public bool ValidarDuplicata(string nome, string telefone, out string mensagem)
        {
            bool nomeDuplicado = vendedorDal.NomeExiste(nome);
            bool telefoneDuplicado = !string.IsNullOrEmpty(telefone) && vendedorDal.TelefoneExiste(telefone); // Ignora validação se telefone estiver vazio

            if (nomeDuplicado && telefoneDuplicado)
            {
                mensagem = "Já existe um vendedor cadastrado com este Nome e Telefone!";
                return true;
            }
            else if (nomeDuplicado)
            {
                mensagem = "Já existe um vendedor cadastrado com este Nome!";
                return true;
            }
            else if (telefoneDuplicado)
            {
                string nomeExistente = vendedorDal.BuscarNomePorTelefone(telefone);
                mensagem = $"O telefone {telefone} já está cadastrado para o vendedor '{nomeExistente}'!";
                return true;
            }
            else
            {
                mensagem = string.Empty;
                return false;
            }
        }

        public string VerificarNomeParecido(string nome)
        {
            return vendedorDal.BuscarNomeParecido(nome);
        }


        private void Log(string message)
        {
            File.AppendAllText("logTavelaVendedor.txt", $"{DateTime.Now}: {message}\n");
        }
        public void Excluir(Model.VendedorMODEL vendedor)
        {
            int entregasPendentes = vendedorDal.ContarEntregasPendentes(vendedor.VendedorID);
            if (entregasPendentes > 0)
            {
                throw new Exception($"Não é possível excluir o vendedor. Há {entregasPendentes} entrega(s) pendente(s) de prestação de contas!");
            }
            vendedorDal.Excluir(vendedor.VendedorID);
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
