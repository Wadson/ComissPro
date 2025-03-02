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
        private string StatusOperacao;
        public FrmManutencaoPrestacaoDeContas(string statusOperacao)
        {
            InitializeComponent();
            this.StatusOperacao = statusOperacao;
            dataGridPrestacaoContas.DataBindingComplete += DataGridPrestacaoContas_DataBindingComplete;
        }
        public void Listar()
        {
            PrestacaoDeContasDAL objetoDAL = new PrestacaoDeContasDAL();
            dataGridPrestacaoContas.DataSource = objetoDAL.listaEntregas();
            PersonalizarDataGridView();
        }

        public void PersonalizarDataGridView()
        {
            if (dataGridPrestacaoContas.Columns.Count >= 9) // Temos 9 colunas
            {
                // Renomeia as colunas na ordem solicitada
                dataGridPrestacaoContas.Columns[0].Name = "NomeVendedor";
                dataGridPrestacaoContas.Columns[1].Name = "QuantidadeEntregue";
                dataGridPrestacaoContas.Columns[2].Name = "NomeProduto";
                dataGridPrestacaoContas.Columns[3].Name = "Preco";
                dataGridPrestacaoContas.Columns[4].Name = "QuantidadeVendida";
                dataGridPrestacaoContas.Columns[5].Name = "QuantidadeDevolvida";
                dataGridPrestacaoContas.Columns[6].Name = "ValorRecebido";
                dataGridPrestacaoContas.Columns[7].Name = "Comissao";
                dataGridPrestacaoContas.Columns[8].Name = "DataPrestacao";

                // Define larguras fixas específicas para as colunas
                dataGridPrestacaoContas.Columns["NomeVendedor"].Width = 200;
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].Width = 140;
                dataGridPrestacaoContas.Columns["NomeProduto"].Width = 200;
                dataGridPrestacaoContas.Columns["Preco"].Width = 120;
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].Width = 140;
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].Width = 140;
                dataGridPrestacaoContas.Columns["ValorRecebido"].Width = 120;
                dataGridPrestacaoContas.Columns["Comissao"].Width = 120;
                dataGridPrestacaoContas.Columns["DataPrestacao"].Width = 130;

                // Define cabeçalhos visíveis
                dataGridPrestacaoContas.Columns["NomeVendedor"].HeaderText = "Vendedor";
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].HeaderText = "Qtd. Entregue";
                dataGridPrestacaoContas.Columns["NomeProduto"].HeaderText = "Produto";
                dataGridPrestacaoContas.Columns["Preco"].HeaderText = "Preço Unitário";
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].HeaderText = "Qtd. Vendida";
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].HeaderText = "Qtd. Devolvida";
                dataGridPrestacaoContas.Columns["ValorRecebido"].HeaderText = "Valor Recebido";
                dataGridPrestacaoContas.Columns["Comissao"].HeaderText = "Comissão";
                dataGridPrestacaoContas.Columns["DataPrestacao"].HeaderText = "Data Prestação";

                // Formatar valores monetários (N2)
                dataGridPrestacaoContas.Columns["Preco"].DefaultCellStyle.Format = "N2";
                dataGridPrestacaoContas.Columns["ValorRecebido"].DefaultCellStyle.Format = "N2";
                dataGridPrestacaoContas.Columns["Comissao"].DefaultCellStyle.Format = "N2";

                // Formatar DataPrestacao como data curta e tratar DBNull
                dataGridPrestacaoContas.Columns["DataPrestacao"].DefaultCellStyle.Format = "d";
                dataGridPrestacaoContas.Columns["DataPrestacao"].DefaultCellStyle.NullValue = "";

                // Centralizar colunas numéricas
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["Preco"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["ValorRecebido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["Comissao"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Centralizar cabeçalhos das colunas
                foreach (DataGridViewColumn column in dataGridPrestacaoContas.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                }

                // Estilizar a linha de totais diretamente
                if (dataGridPrestacaoContas.Rows.Count > 1) // Garantir que há mais de uma linha (dados + total)
                {
                    int ultimaLinha = dataGridPrestacaoContas.Rows.Count - 1;
                    if (dataGridPrestacaoContas.Rows[ultimaLinha].Cells["NomeVendedor"].Value?.ToString() == "TOTAIS")
                    {
                        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.DarkGray;
                        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.ForeColor = Color.White;
                        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dataGridPrestacaoContas.Font, FontStyle.Bold);
                    }
                }
            }

            // Configurações adicionais
            dataGridPrestacaoContas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridPrestacaoContas.ReadOnly = true;
        }

        // Evento para estilizar a linha de totais
        private void DataGridPrestacaoContas_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridPrestacaoContas.Rows.Count > 1) // Garantir que há mais de uma linha
            {
                int ultimaLinha = dataGridPrestacaoContas.Rows.Count - 1;
                if (dataGridPrestacaoContas.Rows[ultimaLinha].Cells["NomeVendedor"].Value?.ToString() == "TOTAIS")
                {
                    dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.DarkGray;
                    dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.ForeColor = Color.White;
                    dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dataGridPrestacaoContas.Font, FontStyle.Bold);
                }
            }
        }
        private void CarregaDados()
        {
            //FrmPrestacaoDeContasDataGrid formPrestacaoContas = new FrmPrestacaoDeContasDataGrid(StatusOperacao);

            //if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO")
            //{
            //    try
            //    {
            //        if (dataGridPrestacaoContas.Rows.Count == 0)
            //        {
            //            MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //            return;
            //        }
            //        formPrestacaoContas.txtNomeVendedor.Text = dataGridPrestacaoContas.CurrentRow.Cells["NomeVendedor"].Value.ToString();
            //        formPrestacaoContas.txtNomeProduto.Text = dataGridPrestacaoContas.CurrentRow.Cells["NomeProduto"].Value.ToString();
            //        formPrestacaoContas.txtQuantidadeEntregue.Text = dataGridPrestacaoContas.CurrentRow.Cells["QuantidadeEntregue"].Value.ToString();
            //        formPrestacaoContas.txtPrecoUnit.Text = dataGridPrestacaoContas.CurrentRow.Cells["Preco"].Value.ToString();
            //        formPrestacaoContas.txtTotal.Text = dataGridPrestacaoContas.CurrentRow.Cells["Total"].Value.ToString();
            //        formPrestacaoContas.dtpDataPrestacaoContas.Text = dataGridPrestacaoContas.CurrentRow.Cells["DataPrestacao"].Value.ToString();
            //        formPrestacaoContas.txtQuantidadeVendida.Text = dataGridPrestacaoContas.CurrentRow.Cells["QuantidadeVendida"].Value.ToString();
            //        formPrestacaoContas.PrestacaoID = int.Parse(dataGridPrestacaoContas.CurrentRow.Cells["PrestacaoID"].Value.ToString());
            //        formPrestacaoContas.EntregaID = int.Parse(dataGridPrestacaoContas.CurrentRow.Cells["EntregaID"].Value.ToString());

            //        if (StatusOperacao == "ALTERAR")
            //        {
            //            formPrestacaoContas.lblStatus.Text = "Alterar";
            //            formPrestacaoContas.lblStatus.ForeColor = Color.Orange;
            //            formPrestacaoContas.btnSalvar.Text = "Alterar";
            //            formPrestacaoContas.btnSalvar.ForeColor = Color.Orange;
            //            formPrestacaoContas.btnSalvar.BackColor = Color.OrangeRed;
            //            formPrestacaoContas.btnNovo.Enabled = false;
            //            formPrestacaoContas.txtNomeVendedor.Enabled = false;
            //            formPrestacaoContas.txtNomeProduto.Enabled = false;                       

            //        }
            //        else if (StatusOperacao == "EXCLUSÃO")
            //        {
            //            formPrestacaoContas.lblStatus.Text = "Exluir registro!";
            //            formPrestacaoContas.lblStatus.ForeColor = Color.Red;
            //            formPrestacaoContas.btnSalvar.Text = "Excluir";                        
            //            formPrestacaoContas.txtNomeVendedor.Enabled = false;
            //            formPrestacaoContas.txtNomeProduto.Enabled = false;
            //            formPrestacaoContas.txtQuantidadeEntregue.Enabled = false;
            //            formPrestacaoContas.txtQuantidadeVendida.Enabled = false;
            //            formPrestacaoContas.txtQtdDevolvida.Enabled = false;
            //            formPrestacaoContas.txtValorRecebido.Enabled = false;
            //            formPrestacaoContas.txtValorComissao.Enabled = false;
            //            formPrestacaoContas.dtpDataPrestacaoContas.Enabled = false;                        
            //            formPrestacaoContas.txtPrecoUnit.Enabled = false;
            //            formPrestacaoContas.txtTotal.Enabled = false;
            //        }

            //        formPrestacaoContas.ShowDialog();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (StatusOperacao == "NOVO")
            //{
            //    formPrestacaoContas.lblStatus.Text = "PRESTAÇÃO DE CONTAS";
            //    formPrestacaoContas.ShowDialog();
            //}
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

        private void FrmManutencaoPrestacaoDeContas_Load(object sender, EventArgs e)
        {
            Listar();
        }
    }
}
