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
            if (dgv.Columns.Count >= 9)
            {
                // Renomeia as colunas
                dgv.Columns[0].Name = "NomeVendedor";
                dgv.Columns[1].Name = "NomeProduto";
                dgv.Columns[2].Name = "QuantidadeEntregue";
                dgv.Columns[3].Name = "Preco";
                dgv.Columns[4].Name = "Total";
                dgv.Columns[5].Name = "DataEntrega";
                dgv.Columns[6].Name = "EntregaID";
                dgv.Columns[7].Name = "VendedorID";
                dgv.Columns[8].Name = "ProdutoID";

                // Define larguras fixas específicas para as colunas
                dgv.Columns["NomeVendedor"].Width = 150;
                dgv.Columns["NomeProduto"].Width = 200;
                dgv.Columns["QuantidadeEntregue"].Width = 100;
                dgv.Columns["Preco"].Width = 120;
                dgv.Columns["Total"].Width = 120;
                dgv.Columns["DataEntrega"].Width = 130;

                // Formatar valores monetários (N2) para Preco e Total
                dgv.Columns["Preco"].DefaultCellStyle.Format = "N2";
                dgv.Columns["Total"].DefaultCellStyle.Format = "N2";

                // Formatar DataEntrega como data curta
                dgv.Columns["DataEntrega"].DefaultCellStyle.Format = "d"; // Formato de data curta (short date)

                // Centralizar a coluna QuantidadeEntregue
                dgv.Columns["QuantidadeEntregue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Ocultar as colunas VendedorID e ProdutoID
                dgv.Columns["VendedorID"].Visible = false;
                dgv.Columns["ProdutoID"].Visible = false;

                // Centralizar cabeçalhos das colunas
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                }
            }

            // Configurações adicionais
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Desativa ajuste automático
            dgv.ReadOnly = true; // Torna o DataGridView somente leitura
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

                    frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["NomeVendedor"].Value.ToString();
                    frm.txtNomeProduto.Text = dataGridPesquisar.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    frm.txtQuantidade.Text = dataGridPesquisar.CurrentRow.Cells["QuantidadeEntregue"].Value.ToString();
                    frm.txtPrecoUnit.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.txtTotal.Text = dataGridPesquisar.CurrentRow.Cells["Total"].Value.ToString();
                    frm.dtpDataEntregaBilhete.Text = dataGridPesquisar.CurrentRow.Cells["DataEntrega"].Value.ToString(); 
                    frm.txtEntregaID.Text = dataGridPesquisar.CurrentRow.Cells["EntregaID"].Value.ToString();
                    frm.VendedorID = int.Parse(dataGridPesquisar.CurrentRow.Cells["VendedorID"].ToString());
                    frm.ProdutoID = int.Parse(dataGridPesquisar.CurrentRow.Cells["ProdutoID"].Value.ToString());


                    frm.lblStatus.Text = "ALTERAR CADASTRO";
                    frm.lblStatus.ForeColor = Color.Orange;
                    StatusOperacao = "ALTERAR";

                    frm.btnNovo.Enabled = false;
                    frm.btnSalvar.Text = "Alterar";

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
                        frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["NomeVendedor"].Value.ToString();
                    frm.txtNomeProduto.Text = dataGridPesquisar.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    frm.txtQuantidade.Text = dataGridPesquisar.CurrentRow.Cells["QuantidadeEntregue"].Value.ToString();
                    frm.txtPrecoUnit.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.txtTotal.Text = dataGridPesquisar.CurrentRow.Cells["Total"].Value.ToString();
                    frm.dtpDataEntregaBilhete.Text = dataGridPesquisar.CurrentRow.Cells["DataEntrega"].Value.ToString();
                    frm.txtEntregaID.Text = dataGridPesquisar.CurrentRow.Cells["EntregaID"].Value.ToString();
                    frm.VendedorID = int.Parse(dataGridPesquisar.CurrentRow.Cells["VendedorID"].ToString());
                    frm.ProdutoID = int.Parse(dataGridPesquisar.CurrentRow.Cells["ProdutoID"].Value.ToString());

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

                    frm.btnSalvar.Text = "Excluir";
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
            EntregasDal objetoDAL = new EntregasDal();
            dataGridPesquisar.DataSource = objetoDAL.listaEntregas();
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
