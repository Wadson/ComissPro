using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
	public partial class FrmManutençãodeEntregaBilhetes : ComissPro.FrmModelo
	{
        private new string StatusOperacao;
        public FrmManutençãodeEntregaBilhetes(string statusOperacao)
		{
			InitializeComponent();
            this.StatusOperacao = statusOperacao;
            //Centraliza o Label dentro do Panel
            label28.Location = new Point(
                (kryptonPanel2.Width - label28.Width) / 2,
                (kryptonPanel2.Height - label28.Height) / 2);
        }
        public void PersonalizarDataGridView(KryptonDataGridView dgv)
        {
            //Alinhar o as colunas
            dgv.Columns["EntregaID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["VendedorID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["ProdutoID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["QuantidadeEntregue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["DataEntrega"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;

            dgv.Columns[0].Name = "EntregaID";
            dgv.Columns[1].Name = "VendedorID";
            dgv.Columns[2].Name = "ProdutoID";
            dgv.Columns[3].Name = "QuantidadeEntregue";
            dgv.Columns[4].Name = "DataEntrega";            

            //dgv.Columns["FornecedorID"].Visible = false;
            //dgv.Columns["CidadeID"].Visible = false;

            // Ajustar colunas automaticamente
            //dataGridPesquisar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Redimensionar as colunas manualmente
            dgv.Columns["EntregaID"].Width = 100;
            dgv.Columns["VendedorID"].Width = 100;
            dgv.Columns["ProdutoID"].Width = 100;
            dgv.Columns["QuantidadeEntregue"].Width = 150;
            dgv.Columns["DataEntrega"].Width = 100;

        }
        private void CarregaDados()
        {
            FrmControleEntregas frm = new FrmControleEntregas(StatusOperacao);

            if (StatusOperacao == "NOVO")
            {
                frm.lblStatus.Text = "NOVA ENTREGA DE BILHETES";
                frm.lblStatus.ForeColor = Color.FromArgb(8, 142, 254);
                frm.ShowDialog();
            }
            if (StatusOperacao == "ALTERAR")
            {
                try
                {
                    // Verificar se a DataGridView contém alguma linha
                    if (dataGridPesquisar.Rows.Count == 0)
                    {
                        // Lançar exceção personalizada
                        //throw new Exception("A DataGridView está vazia. Não há dados para serem processados.");
                        MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    // Execução do código desejado

                    frm.txtEntregaID.Text = dataGridPesquisar.CurrentRow.Cells["ProdutoID"].Value.ToString();
                    frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtNomeProduto.Text = dataGridPesquisar.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    frm.txtPrecoUnit.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.txtQuantidade.Text = dataGridPesquisar.CurrentRow.Cells["Quantidade"].Value.ToString();

                    frm.lblStatus.Text = "ALTERAR CADASTRO";
                    frm.lblStatus.ForeColor = Color.Orange;
                    StatusOperacao = "ALTERAR";

                    frm.btnNovo.Enabled = false;
                    frm.btnConfirmar.Text = "Alterar";

                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Exibir uma mensagem de erro para o usuário
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (StatusOperacao == "EXCLUSÃO")
            {
                try
                {
                    // Verificar se a DataGridView contém alguma linha
                    if (dataGridPesquisar.Rows.Count == 0)
                    {
                        // Lançar exceção personalizada
                        //throw new Exception("A DataGridView está vazia. Não há dados para serem processados.");
                        MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                        // Exemplo: Acessar a primeira célula de cada linha
                        //  var valor = row.Cells[0].Value;
                    frm.txtEntregaID.Text = dataGridPesquisar.CurrentRow.Cells["EntregaID"].Value.ToString();
                    frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtNomeProduto.Text = dataGridPesquisar.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    frm.txtQuantidade.Text = dataGridPesquisar.CurrentRow.Cells["Quantidade"].Value.ToString();
                    frm.txtPrecoUnit.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.txtTotal.Text = dataGridPesquisar.CurrentRow.Cells["Total"].Value.ToString();

                    frm.lblStatus.Text = "EXCLUSÃO DE REGISTRO!";
                    frm.lblStatus.ForeColor = Color.Red;
                    StatusOperacao = "EXCLUSÃO";

                    frm.btnNovo.Enabled = false;


                    frm.txtEntregaID.Enabled = false;
                    frm.txtNomeVendedor.Enabled = false;
                    frm.txtNomeProduto.Enabled = false;
                    frm.txtQuantidade.Enabled = false;
                    frm.txtPrecoUnit.Enabled = false;
                    frm.txtTotal.Enabled = false;

                    frm.btnConfirmar.Text = "Excluir";
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Exibir uma mensagem de erro para o usuário
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void Listar()
        {
            ProdutoDAL objetoDAL = new ProdutoDAL();
            dataGridPesquisar.DataSource = objetoDAL.listarProduto();
            PersonalizarDataGridView(dataGridPesquisar);
        }
        public void HabilitarTimer(bool habilitar)
        {
            timer1.Enabled = habilitar;
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

        private void FrmManutençãodeEntregaBilhetes_Load(object sender, EventArgs e)
        {
            Listar();
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
        }

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.ToLower();

            string nome = "%" + txtPesquisa.Text + "%";
            ProdutoDAL dao = new ProdutoDAL();

            if (rbtCodigo.Checked)
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorCodigo(nome);
                Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
            }
            else
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorNome(nome);
                Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
            }
        }
    }
}
