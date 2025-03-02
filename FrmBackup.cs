using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmBackup : KryptonForm
    {
        public FrmBackup()
        {
            InitializeComponent();
        }
        private void GerarBackup()
        {
            try
            {
                // String de conexão com o banco de dados
                string stringConexao = "Data Source=dbcomisscontrol.db;Version=3;";

                // Extrai o caminho da origem do banco de dados
                string origem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbcomisscontrol.db");

                // Obtém o caminho de destino do TextBox
                string destino = txtDestino.Text;

                // Validar se o caminho de destino foi preenchido
                if (string.IsNullOrEmpty(destino))
                {
                    MessageBox.Show("Por favor, preencha o campo de destino.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar se o arquivo de origem existe
                if (!File.Exists(origem))
                {
                    MessageBox.Show("O banco de dados de origem não foi encontrado no local padrão.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Copiar o arquivo para o destino
                File.Copy(origem, destino, true);

                MessageBox.Show("Backup realizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao gerar o backup: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {            
        }

        private void btnOrigem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos SQLite (*.sqlite;*.db)|*.sqlite;*.db|Todos os arquivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtOrigem.Text = ofd.FileName;
                }
            }
        }

        private void btnDestino_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // Define o nome pré-definido para o backup
                    string nomeArquivoBackup = "backup_banco.sqlite";

                    // Combina o caminho selecionado com o nome do arquivo
                    string caminhoCompleto = Path.Combine(fbd.SelectedPath, nomeArquivoBackup);

                    // Exibe o caminho completo no TextBox de destino
                    txtDestino.Text = caminhoCompleto;
                }
            }
        }

        private void btnGerarBackup_Click(object sender, EventArgs e)
        {
            GerarBackup();
        }

        private void FrmBackup_Load(object sender, EventArgs e)
        {
            // Caminho fixo do banco de dados
            string caminhoOrigem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbcomisscontrol.db");

            // Exibir o caminho no txtOrigem
            txtOrigem.Text = caminhoOrigem;
        }
    }
}
