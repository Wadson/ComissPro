using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmGerarDataPassada : KryptonForm
    {
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("X7K9P2M4Q8J5N3L6");
        public FrmGerarDataPassada()
        {
            InitializeComponent();
        }

        // Método para criptografar a data escolhida
        private string GenerateExpiredDate(DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd HH:mm:ss");

            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = new byte[16]; // Mesmo IV usado no TrialManager
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] dateBytes = Encoding.UTF8.GetBytes(dateString);
                byte[] encrypted = encryptor.TransformFinalBlock(dateBytes, 0, dateBytes.Length);
                return Convert.ToBase64String(encrypted);
            }
        }








       
       
        private void FrmGerarDataPassada_KeyDown(object sender, KeyEventArgs e)
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
                if (MessageBox.Show("Deseja sair?", "Atenção",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void btnGerarDataCriptografad_Click(object sender, EventArgs e)
        {
            string encryptedDate = GenerateExpiredDate(dtpData.Value);
            txtDataCriptografada.Text = encryptedDate;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDataCriptografada.Text))
            {
                Clipboard.SetText(txtDataCriptografada.Text);
                MessageBox.Show("Data criptografada copiada!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

