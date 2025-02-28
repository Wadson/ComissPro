using ComponentFactory.Krypton.Toolkit;
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
    public partial class FrmManutVendedor : KryptonForm
    {
        private new string StatusOperacao;
        public FrmManutVendedor(string statusOperacao)
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
            dgv.Columns["VendedorID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["Cpf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            dgv.Columns["Telefone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;

            dgv.Columns[0].Name = "VendedorID";
            dgv.Columns[1].Name = "Nome";
            dgv.Columns[2].Name = "Cpf";            
            dgv.Columns[3].Name = "Telefone";
            dgv.Columns[4].Name = "Comissao";

            //dgv.Columns["FornecedorID"].Visible = false;
            //dgv.Columns["CidadeID"].Visible = false;

            // Ajustar colunas automaticamente
            //dataGridPesquisar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Redimensionar as colunas manualmente
            dgv.Columns["VendedorID"].Width = 100;
            dgv.Columns["Nome"].Width = 400;
            dgv.Columns["Cpf"].Width = 150;
            dgv.Columns["Telefone"].Width = 150;

        }
        private void CarregaDados()
        {
            FrmCadVendedor frm = new FrmCadVendedor(StatusOperacao);

            if (StatusOperacao == "NOVO")
            {
                frm.lblStatus.Text = "Novo cadastro de vendedores";
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

                    frm.txtVendedorID.Text = dataGridPesquisar.CurrentRow.Cells["VendedorID"].Value.ToString();
                    frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtCpf.Text = dataGridPesquisar.CurrentRow.Cells["Cpf"].Value.ToString();                    
                    frm.txtTelefone.Text = dataGridPesquisar.CurrentRow.Cells["Telefone"].Value.ToString();

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
                    frm.txtVendedorID.Text = dataGridPesquisar.CurrentRow.Cells["VendedorID"].Value.ToString();
                    frm.txtNomeVendedor.Text = dataGridPesquisar.CurrentRow.Cells["Nome"].Value.ToString();
                    frm.txtCpf.Text = dataGridPesquisar.CurrentRow.Cells["Cpf"].Value.ToString();
                    frm.txtTelefone.Text = dataGridPesquisar.CurrentRow.Cells["Telefone"].Value.ToString();

                    frm.lblStatus.Text = "EXCLUSÃO DE REGISTRO!";
                    frm.lblStatus.ForeColor = Color.Red;
                    StatusOperacao = "EXCLUSÃO";

                    frm.btnNovo.Enabled = false;


                    frm.txtVendedorID.Enabled = false;
                    frm.txtNomeVendedor.Enabled = false;
                    frm.txtCpf.Enabled = false;
                    frm.txtTelefone.Enabled = false;
                    
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
        private void btnNovo_Click(object sender, EventArgs e)
        {
            StatusOperacao = "NOVO";
            CarregaDados();
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
        public void Listar()
        {
            VendedorDAL objetoDAL = new VendedorDAL();
            dataGridPesquisar.DataSource = objetoDAL.listaVendedor();
            PersonalizarDataGridView(dataGridPesquisar);
        }

        private void FrmManutVendedor_Load(object sender, EventArgs e)
        {
            Listar();
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
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

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.ToLower();

            string nome = "%" + txtPesquisa.Text + "%";
            VendedorDAL dao = new VendedorDAL();

            dataGridPesquisar.DataSource = dao.PesquisarPorNome(nome);
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPesquisar);
        }
    }
}
