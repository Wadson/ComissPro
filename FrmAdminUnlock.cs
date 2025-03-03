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
using System.Security.Cryptography;

namespace ComissPro
{
    public partial class FrmAdminUnlock : KryptonForm
    {
        // Hash SHA1 da senha "ComissPro2025" (gere o hash real e substitua)
        private const string AdminPasswordHash = "6d989106a4935db0bed7df621d4712a001cc4faf";//Senha usada pelo administrador para desbloqueio do sistema
        public FrmAdminUnlock()
        {
            InitializeComponent();
            lblInstruction.Text = "Digite a senha de administrador \npara desbloquear o sistema permanentemente:";
        }      
       

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            string inputPassword = txtPassword.Text;
            string inputHash = ComputeSha1Hash(inputPassword);

            if (inputHash == AdminPasswordHash)
            {
                TrialManager.SetLicensed();
                MessageBox.Show("Sistema desbloqueado com sucesso! Agora funciona sem limite de tempo.",
                    "Desbloqueio Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Senha incorreta. Tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
            }
        }
        private string ComputeSha1Hash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
