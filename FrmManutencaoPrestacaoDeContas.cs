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
    public partial class FrmManutencaoPrestacaoDeContas : KryptonForm
    {
        private new string StatusOperacao;
        public FrmManutencaoPrestacaoDeContas(string statusOperacao)
        {
            InitializeComponent();
            this.StatusOperacao = statusOperacao;    
        }
        public void Listar()
        {
            PrestacaoDeContasDAL objetoDAL = new PrestacaoDeContasDAL();
            dataGridPrestacaoContas.DataSource = objetoDAL.listaEntregas();
            //PersonalizarDataGridView(dataGridPrestacaoContas);
        }
        private void CarregaDados()
        {
            FrmPrestacaoDeContas formPrestacaoContas = new FrmPrestacaoDeContas(StatusOperacao);

            if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO")
            {
                try
                {
                    if (dataGridPrestacaoContas.Rows.Count == 0)
                    {
                        MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    formPrestacaoContas.txtNomeVendedor.Text = dataGridPrestacaoContas.CurrentRow.Cells["NomeVendedor"].Value.ToString();
                    formPrestacaoContas.txtNomeProduto.Text = dataGridPrestacaoContas.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    formPrestacaoContas.txtQuantidadeEntregue.Text = dataGridPrestacaoContas.CurrentRow.Cells["QuantidadeEntregue"].Value.ToString();
                    formPrestacaoContas.txtPrecoUnit.Text = dataGridPrestacaoContas.CurrentRow.Cells["Preco"].Value.ToString();
                    formPrestacaoContas.txtTotal.Text = dataGridPrestacaoContas.CurrentRow.Cells["Total"].Value.ToString();
                    formPrestacaoContas.dtpDataPrestacaoContas.Text = dataGridPrestacaoContas.CurrentRow.Cells["DataPrestacao"].Value.ToString();
                    formPrestacaoContas.txtQuantidadeVendida.Text = dataGridPrestacaoContas.CurrentRow.Cells["QuantidadeVendida"].Value.ToString();
                    formPrestacaoContas.txtPrestacaoID.Text = dataGridPrestacaoContas.CurrentRow.Cells["PrestacaoID"].Value.ToString();
                    formPrestacaoContas.EntregaID = int.Parse(dataGridPrestacaoContas.CurrentRow.Cells["EntregaID"].Value.ToString());

                    if (StatusOperacao == "ALTERAR")
                    {
                        formPrestacaoContas.lblStatus.Text = "Alterar";
                        formPrestacaoContas.lblStatus.ForeColor = Color.Orange;
                        formPrestacaoContas.btnSalvar.Text = "Alterar";
                        formPrestacaoContas.btnSalvar.ForeColor = Color.Orange;
                        formPrestacaoContas.btnSalvar.BackColor = Color.OrangeRed;
                        formPrestacaoContas.btnNovo.Enabled = false;
                        formPrestacaoContas.txtNomeVendedor.Enabled = false;
                        formPrestacaoContas.txtNomeProduto.Enabled = false;                       

                    }
                    else if (StatusOperacao == "EXCLUSÃO")
                    {
                        formPrestacaoContas.lblStatus.Text = "Exluir registro!";
                        formPrestacaoContas.lblStatus.ForeColor = Color.Red;
                        formPrestacaoContas.btnSalvar.Text = "Excluir";
                        formPrestacaoContas.txtPrestacaoID.Enabled = false;
                        formPrestacaoContas.txtNomeVendedor.Enabled = false;
                        formPrestacaoContas.txtNomeProduto.Enabled = false;
                        formPrestacaoContas.txtQuantidadeEntregue.Enabled = false;
                        formPrestacaoContas.txtQuantidadeVendida.Enabled = false;
                        formPrestacaoContas.txtQtdDevolvida.Enabled = false;
                        formPrestacaoContas.txtValorRecebido.Enabled = false;
                        formPrestacaoContas.txtValorComissao.Enabled = false;
                        formPrestacaoContas.dtpDataPrestacaoContas.Enabled = false;                        
                        formPrestacaoContas.txtPrecoUnit.Enabled = false;
                        formPrestacaoContas.txtTotal.Enabled = false;
                    }

                    formPrestacaoContas.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (StatusOperacao == "NOVO")
            {
                formPrestacaoContas.lblStatus.Text = "PRESTAÇÃO DE CONTAS";
                formPrestacaoContas.ShowDialog();
            }
        }
        public void HabilitarTimer(bool habilitar)
        {
            timer1.Enabled = habilitar;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Listar();
            timer1.Enabled = false;
        }

        private void FrmManutencaoPrestacaoDeContas_KeyDown(object sender, KeyEventArgs e)
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

        private void btnNovo_Click(object sender, EventArgs e)
        {
            StatusOperacao = "NOVO";
            CarregaDados();
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            StatusOperacao = "ALTERAR";
            CarregaDados();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            StatusOperacao = "EXCLUSÃO";
            CarregaDados();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
