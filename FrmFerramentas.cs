using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmFerramentas : KryptonForm
    {
        public FrmFerramentas()
        {
            InitializeComponent();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGerarBackup_Click(object sender, EventArgs e)
        {
            FrmBackup frmBackup = new FrmBackup();
            frmBackup.ShowDialog();
        }

        private void btnRestaurarBackup_Click(object sender, EventArgs e)
        {
            FrmRestaurarBackup frmRestaurarBackup = new FrmRestaurarBackup();
            frmRestaurarBackup.ShowDialog();
        }

        private void btnExcluirOrfaos_Click(object sender, EventArgs e)
        {
            PrestacaoDeContasDAL prestacaoDeContasDAL = new PrestacaoDeContasDAL();
            try
            {
                int excluidas = prestacaoDeContasDAL.ExcluirContasOrfas();
                MessageBox.Show($"{excluidas} contas órfãs foram excluídas com sucesso.",
                                "Sucesso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnGerarHasSh2_Click(object sender, EventArgs e)
        {
            FrmGeradorHas frmGeradorHas = new FrmGeradorHas();
            frmGeradorHas.ShowDialog();
        }

        private void btnGerarDataPassadaCriptografada_Click(object sender, EventArgs e)
        {
            FrmGerarDataPassada frmGerarDataPassada = new FrmGerarDataPassada();
            frmGerarDataPassada.ShowDialog();
        }
    }
}
