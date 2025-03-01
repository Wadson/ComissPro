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
    public partial class FrmRestaurarBackup : KryptonForm
    {
        public FrmRestaurarBackup()
        {
            InitializeComponent();
        }
        private void RestaurarBancoDeDados()
        {
            try
            {
                // Caminho do destino automático (diretório raiz do programa)
                string destino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbcomisscontrol.db");

                // Obter o caminho de origem do TextBox
                string origem = txtOrigem.Text;

                // Validar se o caminho de origem foi preenchido
                if (string.IsNullOrEmpty(origem))
                {
                    MessageBox.Show("Por favor, preencha o campo de origem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar se o arquivo de origem existe
                if (!File.Exists(origem))
                {
                    MessageBox.Show("O arquivo de backup especificado não foi encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Copiar o arquivo de origem para o destino
                File.Copy(origem, destino, true);

                MessageBox.Show("Restauração realizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao restaurar o backup: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOrigem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos SQLite (*.sqlite;*.db)|*.sqlite;*.db|Todos os arquivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Exibir o caminho do arquivo selecionado no txtOrigem
                    txtOrigem.Text = ofd.FileName;
                }
            }
        }

        private void FrmRestaurarBackup_Load(object sender, EventArgs e)
        {
            // Caminho fixo do destino para restauração (pasta raiz do programa)
            string caminhoDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbcomisscontrol.db");

            // Exibir o caminho no txtDestino
            txtDestino.Text = caminhoDestino;

            // Bloquear edição apenas para visualização (se não estiver configurado ainda)
            txtDestino.ReadOnly = true;
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            RestaurarBancoDeDados();
        }
    }
}
