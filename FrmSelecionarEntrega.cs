using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ComissPro
{
    public partial class FrmSelecionarEntrega : KryptonForm
    {
        
        protected int LinhaAtual = -1;//Foi retirado pode comentar
        public int VendedorID { get; set; }
        public string Nome { get; set; }
        public string vendedorSelecionado { get; set; }



        public int? VendedorSelecionadoID { get; private set; } // Substitui EntregaSelecionadaID


        public FrmSelecionarEntrega(Form formChamador, string textoDigitado)
        {
            InitializeComponent();

            txtPesquisa.Text = textoDigitado;
            txtPesquisa.SelectionStart = txtPesquisa.Text.Length;
            txtPesquisa.Focus();
            txtPesquisa.KeyDown += txtPesquisa_KeyDown;
            dgvEntregasPendentes.KeyDown += dgvEntregasPendentes_KeyDown;
            this.Text = "Selecionar Vendedor com Entregas Pendentes";
            CarregarEntregasPendentes();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        // No FrmLocalizarProduto, após selecionar o produto e fechar o formulário
        private bool isSelectingVendedor = false;
        

        public new int ObterLinhaAtual()
        {
            return LinhaAtual;
        }
        private void CarregarEntregasPendentes(string filtro = "")
        {
            try
            {
                var entregas = new EntregasDal().CarregarEntregasNaoPrestadas(filtro);
                if (entregas == null || entregas.Count == 0)
                {
                    dgvEntregasPendentes.DataSource = null;
                    MessageBox.Show("Nenhuma entrega encontrada!", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Agrupar por vendedor para mostrar apenas um registro por vendedor
                var vendedores = entregas
                    .GroupBy(e => e.VendedorID)
                    .Select(g => new
                    {
                        VendedorID = g.Key,
                        Nome = g.First().Nome,
                        TotalEntregas = g.Count(),
                        TotalQuantidade = g.Sum(e => e.QuantidadeEntregue)
                    }).ToList();

                DataTable dt = new DataTable();
                dt.Columns.Add("VendedorID", typeof(int));
                dt.Columns.Add("Nome", typeof(string));
                dt.Columns.Add("TotalEntregas", typeof(int));
                dt.Columns.Add("TotalQuantidade", typeof(int));

                foreach (var vendedor in vendedores)
                {
                    dt.Rows.Add(vendedor.VendedorID, vendedor.Nome, vendedor.TotalEntregas, vendedor.TotalQuantidade);
                }

                dgvEntregasPendentes.DataSource = dt;

                // Configurar colunas
                dgvEntregasPendentes.Columns["VendedorID"].Visible = false;
                dgvEntregasPendentes.Columns["Nome"].HeaderText = "Vendedor";
                dgvEntregasPendentes.Columns["TotalEntregas"].HeaderText = "Nº de Entregas";
                dgvEntregasPendentes.Columns["TotalQuantidade"].HeaderText = "Total Bilhetes";

                // Ajustar larguras
                dgvEntregasPendentes.Columns["Nome"].Width = 250;
                dgvEntregasPendentes.Columns["TotalEntregas"].Width = 100;
                dgvEntregasPendentes.Columns["TotalQuantidade"].Width = 100;

                // Alinhamento
                dgvEntregasPendentes.Columns["TotalEntregas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvEntregasPendentes.Columns["TotalQuantidade"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                foreach (DataGridViewColumn column in dgvEntregasPendentes.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.True;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar entregas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SelecionarVendedor()
        {
            if (dgvEntregasPendentes.SelectedRows.Count > 0)
            {
                VendedorSelecionadoID = Convert.ToInt32(dgvEntregasPendentes.SelectedRows[0].Cells["VendedorID"].Value);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Selecione um vendedor para continuar!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            CarregarEntregasPendentes(txtPesquisa.Text.Trim());
        }
        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            SelecionarVendedor();
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            VendedorSelecionadoID = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && dgvEntregasPendentes.Rows.Count > 0)
            {
                dgvEntregasPendentes.Focus();
                dgvEntregasPendentes.CurrentCell = dgvEntregasPendentes.Rows[0].Cells["Nome"];
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SelecionarVendedor();
            }
        }

        private void dgvEntregasPendentes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && dgvEntregasPendentes.CurrentCell?.RowIndex == 0)
            {
                txtPesquisa.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SelecionarVendedor();
            }
        }       
    }
}
