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
	public partial class FrmManutençãodeEntregaBilhetes : KryptonForm
	{
        private bool bloqueiaEventosTextChanged = false;

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
                dgv.Columns["NomeVendedor"].Width = 250;
                dgv.Columns["NomeProduto"].Width = 200;
                dgv.Columns["QuantidadeEntregue"].Width = 120;
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
            FrmControleEntregas formEntregas = new FrmControleEntregas(StatusOperacao);

            if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO")
            {
                try
                {
                    if (dataGridManutencaoEntregas.Rows.Count == 0)
                    {
                        MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    // Bloqueia os eventos temporariamente
                    formEntregas.bloqueiaEventosTextChanged = true;

                    formEntregas.txtNomeVendedor.Text = dataGridManutencaoEntregas.CurrentRow.Cells["NomeVendedor"].Value.ToString();
                    formEntregas.txtNomeProduto.Text = dataGridManutencaoEntregas.CurrentRow.Cells["NomeProduto"].Value.ToString();
                    formEntregas.txtQuantidade.Text = dataGridManutencaoEntregas.CurrentRow.Cells["QuantidadeEntregue"].Value.ToString();
                    formEntregas.txtPrecoUnit.Text = dataGridManutencaoEntregas.CurrentRow.Cells["Preco"].Value.ToString();
                    formEntregas.txtTotal.Text = dataGridManutencaoEntregas.CurrentRow.Cells["Total"].Value.ToString();
                    formEntregas.dtpDataEntregaBilhete.Text = dataGridManutencaoEntregas.CurrentRow.Cells["DataEntrega"].Value.ToString();
                    formEntregas.txtEntregaID.Text = dataGridManutencaoEntregas.CurrentRow.Cells["EntregaID"].Value.ToString();
                    formEntregas.VendedorID = int.Parse(dataGridManutencaoEntregas.CurrentRow.Cells["VendedorID"].Value.ToString());
                    formEntregas.ProdutoID = int.Parse(dataGridManutencaoEntregas.CurrentRow.Cells["ProdutoID"].Value.ToString());

                    if (StatusOperacao == "ALTERAR")
                    {
                        formEntregas.lblStatus.Text = "Alterar: quantidade e valor";
                        formEntregas.lblStatus.ForeColor = Color.Orange;
                        formEntregas.btnSalvar.Text = "Alterar";
                        formEntregas.btnSalvar.ForeColor = Color.Orange;
                        formEntregas.btnSalvar.BackColor = Color.OrangeRed;
                        formEntregas.btnNovo.Enabled = false;
                        formEntregas.txtNomeVendedor.Enabled = false;
                        formEntregas.txtNomeProduto.Enabled = false;
                        formEntregas.btnLocalizarVendedor.Enabled = false;
                        formEntregas.btnLocalizarProduto.Enabled = false;

                    }
                    else if (StatusOperacao == "EXCLUSÃO")
                    {
                        formEntregas.lblStatus.Text = "Exluir registro!";
                        formEntregas.lblStatus.ForeColor = Color.Red;
                        formEntregas.btnSalvar.Text = "Excluir";

                        formEntregas.txtEntregaID.Enabled = false;
                        formEntregas.txtNomeVendedor.Enabled = false;
                        formEntregas.txtNomeProduto.Enabled = false;
                        formEntregas.txtQuantidade.Enabled = false;
                        formEntregas.txtPrecoUnit.Enabled = false;
                        formEntregas.txtTotal.Enabled = false;
                    }

                    // Reativa os eventos após preencher os dados
                    formEntregas.bloqueiaEventosTextChanged = false;

                    formEntregas.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (StatusOperacao == "NOVO")
            {
                formEntregas.lblStatus.Text = "ENTREGA DE BILHETES";                
                formEntregas.ShowDialog();
            }
        }


        public void Listar()
        {
            EntregasDal objetoDAL = new EntregasDal();
            dataGridManutencaoEntregas.DataSource = objetoDAL.listaEntregas();
            PersonalizarDataGridView(dataGridManutencaoEntregas);
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
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridManutencaoEntregas);
        }

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.ToLower();

            string nome = "%" + txtPesquisa.Text + "%";
            ProdutoDAL dao = new ProdutoDAL();

            if (rbtCodigo.Checked)
            {
                dataGridManutencaoEntregas.DataSource = dao.PesquisarPorCodigo(nome);
                Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridManutencaoEntregas);
            }
            else
            {
                dataGridManutencaoEntregas.DataSource = dao.PesquisarPorNome(nome);
                Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridManutencaoEntregas);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Listar();
            timer1.Enabled = false;
        }
    }
}
