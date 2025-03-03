using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmGeradorHas : KryptonForm
    {
        
        public FrmGeradorHas()
        {
            InitializeComponent();
            this.Text = "Gerador de Hash SHA1"; // Título do formulário
        }

        // Método para calcular o hash SHA1
        private string ComputeSha1Hash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void FrmGeradorHas_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {

                if (this.GetNextControl(ActiveControl, true) != null)
                {
                    e.Handled = true;
                    this.GetNextControl(ActiveControl, true).Focus();
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                //this.Close();
                if (MessageBox.Show("Deseja sair?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGerarHasSh_Click(object sender, EventArgs e)
        {
            string input = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Digite uma senha para gerar o hash!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtPassWordHasGerado.Text = ComputeSha1Hash(input);
        }

        // Adicionando opção para copiar o hash para a área de transferência (opcional)
        private void btnCopiar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassWordHasGerado.Text))
            {
                Clipboard.SetText(txtPassWordHasGerado.Text);
                MessageBox.Show("Hash copiado para a área de transferência!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nenhum hash gerado para copiar!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
