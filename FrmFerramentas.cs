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
        public void ExcluirEntregasOrfasEAtualizar()
        {
            LogUtil.Registrar("Usuário solicitou exclusão de registros órfãos.");
            DialogResult resultado = MessageBox.Show("Deseja realmente excluir todas as entregas e prestações de contas órfãs? Essa ação não pode ser desfeita.",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    VendedorBLL vendedorBll = new VendedorBLL();
                    var (entregasExcluidas, prestacoesExcluidas) = vendedorBll.ExcluirRegistrosOrfaos();

                    if (entregasExcluidas > 0 || prestacoesExcluidas > 0)
                    {
                        string mensagem = $"Limpeza concluída com sucesso!\n" +
                                          $"- Entregas excluídas: {entregasExcluidas}\n" +
                                          $"- Prestações de contas excluídas: {prestacoesExcluidas}";
                        MessageBox.Show(mensagem, "Limpeza Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogUtil.Registrar($"Resultado exibido ao usuário: {mensagem}");
                    }
                    else
                    {
                        MessageBox.Show("Nenhum registro órfão encontrado para exclusão.",
                            "Sem Alterações", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogUtil.Registrar("Nenhum registro órfão encontrado para exclusão.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir registros órfãos: " + ex.Message,
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogUtil.Registrar($"Erro exibido ao usuário: {ex.Message}");
                }
            }
            else
            {
                LogUtil.Registrar("Usuário cancelou a exclusão de registros órfãos.");
            }
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

        private void btnExcluirOrfao_Click(object sender, EventArgs e)
        {
            ExcluirEntregasOrfasEAtualizar();
        }
    }
}
