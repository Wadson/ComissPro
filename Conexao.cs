using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComissPro
{
    internal class Conexao
    {
        // Obtém a string de conexão dinamicamente para SQLite
        private static string GetConnectionString()
        {
            // String de conexão para SQLite
            string connString = "Data Source=dbcomisscontrol.db;Version=3;";
                                           //dbcomisscontrol

            // Carrega a configuração do App.config
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.ConnectionStrings.ConnectionStrings["ConexaoDB"];

            if (settings == null)
            {
                // Adiciona a string de conexão para SQLite
                config.ConnectionStrings.ConnectionStrings.Add(
                    new ConnectionStringSettings("ConexaoDB", connString, "System.Data.SQLite")
                );
            }
            else if (settings.ConnectionString != connString)
            {
                // Atualiza a string de conexão caso esteja incorreta
                settings.ConnectionString = connString;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");

            return connString;
        }

        public static SQLiteConnection Conex()
        {
            try
            {
                string conn = GetConnectionString(); // Obtém a string de conexão
                SQLiteConnection myConn = new SQLiteConnection(conn);
                return myConn;
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Erro ao conectar ao banco de dados: " + ex.Message);
            }
        }
    }
}
