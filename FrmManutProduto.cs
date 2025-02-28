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
    public partial class FrmManutProduto : KryptonForm
    {
        private new string StatusOperacao;
        public FrmManutProduto(string statusOperacao)
        {
            this.StatusOperacao = statusOperacao;
            InitializeComponent();
            //Centraliza o Label dentro do Panel
            label28.Location = new Point(
                (kryptonPanel2.Width - label28.Width) / 2,
                (kryptonPanel2.Height - label28.Height) / 2);
        }
        public void PersonalizarDataGridView(KryptonDataGridView dgv)
        {
            //Alinhar o as colunas
            dgv.Columns["ProdutoID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["Preco"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["QuantidadePorBloco"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;


            dgv.Columns[0].Name = "ProdutoID";
            dgv.Columns[1].Name = "Nome";
            dgv.Columns[2].Name = "Preco";
            dgv.Columns[3].Name = "Tipo";
            dgv.Columns[2].DefaultCellStyle.Format = "C2";

            //dgv.Columns["FornecedorID"].Visible = false;
            //dgv.Columns["CidadeID"].Visible = false;

            // Ajustar colunas automaticamente
            //dataGridPesquisar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Redimensionar as colunas manualmente
            dgv.Columns["ProdutoId"].Width = 100;
            dgv.Columns["Nome"].Width = 400;
            dgv.Columns["Preco"].Width = 150;
            dgv.Columns["Tipo"].Width = 150;

        }
        private void CarregaDados()
        {
            FrmCadProdutos frm = new FrmCadProdutos(StatusOperacao);

            if (StatusOperacao == "NOVO")
            {
                frm.lblStatus.Text = "NOVO CADASTRO DE PRODUTOS";
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

                    frm.txtProdutoID.Text = dataGridPesquisar.CurrentRow.Cells["ProdutoID"].Value.ToString();
                    frm.txtNome.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtPreco.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.cmbTipo.Text = dataGridPesquisar.CurrentRow.Cells["Tipo"].Value.ToString();

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
                    frm.txtProdutoID.Text = dataGridPesquisar.CurrentRow.Cells["ProdutoID"].Value.ToString();
                    frm.txtNome.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtPreco.Text = dataGridPesquisar.CurrentRow.Cells["Preco"].Value.ToString();
                    frm.cmbTipo.Text = dataGridPesquisar.CurrentRow.Cells["Tipo"].Value.ToString();

                    frm.lblStatus.Text = "EXCLUSÃO DE REGISTRO!";
                    frm.lblStatus.ForeColor = Color.Red;
                    StatusOperacao = "EXCLUSÃO";

                    frm.btnNovo.Enabled = false;


                    frm.txtProdutoID.Enabled = false;
                    frm.txtNome.Enabled = false;
                    frm.txtPreco.Enabled = false;
                    frm.cmbTipo.Enabled = false;

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
            ProdutoDAL objetoDAL = new ProdutoDAL();
            dataGridPesquisar.DataSource = objetoDAL.listarProduto();
            PersonalizarDataGridView(dataGridPesquisar);
        }
        private void FrmManutProduto_Load(object sender, EventArgs e)
        {
            Listar();
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
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
        public void HabilitarTimer(bool habilitar)
        {
            timer1.Enabled = habilitar;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Listar();
            timer1.Enabled = false;
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
