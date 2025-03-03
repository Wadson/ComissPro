using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ComissPro
{
    internal class Conexao
    {
        private static string _databasePath = "dbcomisscontrol.db"; // Banco no diretório da aplicação
        private static string _connectionString = $"Data Source={_databasePath};Version=3;";

        // Método para inicializar o banco (chamado automaticamente ao obter a conexão)
        private static void InitializeDatabase()
        {
            try
            {
                // Verifica se o banco já existe
                if (!File.Exists(_databasePath))
                {
                    // Cria o arquivo de banco de dados
                    SQLiteConnection.CreateFile(_databasePath);

                    // Cria as tabelas                    
                    if (!File.Exists(_databasePath))
                    {
                        SQLiteConnection.CreateFile(_databasePath);
                        CreateTables();
                        MessageBox.Show("Banco de dados criado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inicializar o banco de dados: " + ex.Message);
            }
        }

        // Cria as tabelas no banco recém-criado
        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Tabela Produtos
                        using (var cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = @"
                                CREATE TABLE Produtos (
                                    ProdutoID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    NomeProduto TEXT NOT NULL,
                                    Preco REAL NOT NULL,
                                    Unidade TEXT NOT NULL
                                )";
                            cmd.ExecuteNonQuery();
                        }

                        // Tabela Vendedores
                        using (var cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = @"
                                CREATE TABLE Vendedores (
                                    VendedorID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    Nome TEXT NOT NULL,
                                    CPF TEXT NULL,
                                    Telefone TEXT NULL,
                                    Comissao REAL DEFAULT (10) NULL
                                )";
                            cmd.ExecuteNonQuery();
                        }

                        // Tabela Entregas
                        using (var cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = @"
                                CREATE TABLE Entregas (
                                    EntregaID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    VendedorID BIGINT NOT NULL,
                                    ProdutoID BIGINT NOT NULL,
                                    QuantidadeEntregue BIGINT NOT NULL,
                                    DataEntrega DATETIME DEFAULT (CURRENT_TIMESTAMP) NULL,
                                    PrestacaoRealizada BIGINT DEFAULT (0) NULL,
                                    CONSTRAINT FK_Entregas_0_0 FOREIGN KEY (VendedorID) REFERENCES Vendedores (VendedorID) ON DELETE NO ACTION ON UPDATE NO ACTION,
                                    CONSTRAINT FK_Entregas_1_0 FOREIGN KEY (ProdutoID) REFERENCES Produtos (ProdutoID) ON DELETE NO ACTION ON UPDATE NO ACTION
                                )";
                            cmd.ExecuteNonQuery();
                        }

                        // Tabela PrestacaoContas
                        using (var cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = @"
                                CREATE TABLE PrestacaoContas (
                                    PrestacaoID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    EntregaID BIGINT NOT NULL,
                                    QuantidadeVendida BIGINT NOT NULL,
                                    QuantidadeDevolvida BIGINT NOT NULL,
                                    ValorRecebido REAL NOT NULL,
                                    Comissao REAL NOT NULL,
                                    DataPrestacao DATETIME DEFAULT (CURRENT_TIMESTAMP) NULL,
                                    CONSTRAINT FK_PrestacaoContas_0_0 FOREIGN KEY (EntregaID) REFERENCES Entregas (EntregaID) ON DELETE NO ACTION ON UPDATE NO ACTION
                                )";
                            cmd.ExecuteNonQuery();
                        }

                        // Tabela FluxoCaixa
                        using (var cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = @"
                                CREATE TABLE FluxoCaixa (
                                    FluxoCaixaID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    TipoMovimentacao TEXT NOT NULL,
                                    Valor REAL NOT NULL,
                                    DataMovimentacao DATETIME DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
                                    Descricao TEXT NULL,
                                    PrestacaoID BIGINT NULL,
                                    CONSTRAINT FK_FluxoCaixa_0_0 FOREIGN KEY (PrestacaoID) REFERENCES PrestacaoContas (PrestacaoID) ON DELETE NO ACTION ON UPDATE NO ACTION
                                )";
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao criar tabelas: " + ex.Message);
                    }
                }
            }
        }

        // Obtém a string de conexão dinamicamente para SQLite
        private static string GetConnectionString()
        {
            // Inicializa o banco antes de retornar a string de conexão
            InitializeDatabase();

            // String de conexão para SQLite
            string connString = _connectionString;

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
                string conn = GetConnectionString(); // Obtém a string de conexão (e inicializa o banco, se necessário)
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