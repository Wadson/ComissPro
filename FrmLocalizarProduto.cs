using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using MetroFramework.Forms;

namespace ComissPro
{
    public partial class FrmLocalizarProduto : ComissPro.FrmModelo
    {
        protected int LinhaAtual = -1;
        public int ProdutoID { get; set; }
        public string Nome { get; set; }
        private decimal Preco;
        private string Tipo;
        private int QuantidadePorBloco;
        public string produtoSelecionado { get; set; }
        public Form FormChamador { get; set; }
        public FrmLocalizarProduto(Form formChamador, string textoDigitado)
        {
            InitializeComponent();
        }

        // No FrmLocalizarProduto, após selecionar o produto e fechar o formulário
        private bool isSelectingProduct = false;
        private Form formChamador;

        public void PersonalizarDataGridView(KryptonDataGridView dgv)
        {
            // Renomear colunas
            dgv.Columns[0].Name = "ProdutoID";
            dgv.Columns[1].Name = "Nome";
            dgv.Columns[2].Name = "Preco";
            dgv.Columns[3].Name = "Tipo";            

            // Ajustar colunas automaticamente
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Tornar o grid somente leitura
            dgv.ReadOnly = true;

            // Centralizar colunas de IDs e Quantidade
            dgv.Columns["ProdutoID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;            

            dgv.Columns["Preco"].DefaultCellStyle.Font = new Font("Arial", 10F, FontStyle.Italic); // Fonte Arial, 10, Italic
            dgv.Columns["Preco"].DefaultCellStyle.ForeColor = System.Drawing.Color.DarkGreen; // Cor da fonte: Verde Escuro
            dgv.Columns["Preco"].DefaultCellStyle.Format = "N2"; // Formato de moeda
            dgv.Columns["Preco"].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue; // Cor de fundo Azul Claro
            dgv.Columns["Preco"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Alinhamento à direita

            // Ocultar a coluna ProdutoID
            dgv.Columns["ProdutoID"].Visible = false;

            // Configurar fundo amarelo claro
            dgv.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow; // Fundo amarelo claro

            // Ajustar largura do cabeçalho da linha
            dgv.RowHeadersWidth = 10; // Definir largura do cabeçalho da linha
                                      // Ajustar largura dos cabeçalhos das colunas
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Centralizar cabeçalho da coluna
                column.HeaderCell.Style.WrapMode = DataGridViewTriState.False; // Evitar quebra de texto no cabeçalho
                column.Width = 100; // Definir largura específica para cada coluna
            }
        }
       
        private void SelecionarProduto()
        {
            // Verifica se o processo de seleção de produto já está em andamento
            if (isSelectingProduct) return;
            isSelectingProduct = true;

            try
            {
                // Obtém a linha atual selecionada na grid
                LinhaAtual = ObterLinhaAtual();
                if (LinhaAtual < 0 || LinhaAtual >= dataGridPesquisar.Rows.Count)
                {
                    // Verifica se a linha obtida é válida
                    MessageBox.Show("Linha inválida.");
                    return;
                }
                // Verifica e obtém os valores das células NomeProduto e PrecoDeVenda
                if (dataGridPesquisar["Nome", LinhaAtual]?.Value == null ||
                    dataGridPesquisar["Preco", LinhaAtual]?.Value == null ||
                    !decimal.TryParse(dataGridPesquisar["Preco", LinhaAtual].Value.ToString(), out Preco))
                {
                    // Caso os valores não sejam válidos, exibe uma mensagem de erro
                    MessageBox.Show("Dados do produto inválidos.");
                    return;
                }

                // Converte o valor da célula NomeProduto para string
                ProdutoID = int.Parse(dataGridPesquisar["ProdutoID", LinhaAtual].Value.ToString());
                Nome = dataGridPesquisar["Nome", LinhaAtual].Value.ToString();
                Preco = Decimal.Parse(dataGridPesquisar["Preco", LinhaAtual].Value.ToString());
                Tipo = dataGridPesquisar["Tipo", LinhaAtual].Value.ToString();
                QuantidadePorBloco = int.Parse(dataGridPesquisar["QuantidadePorBloco", LinhaAtual].Value.ToString());

                // Acrescenta zeros à esquerda do ProdutoID
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(ProdutoID, 4);

                // Obtém a instância do formulário FrmPedido (ou usa uma existente)
                if (this.Owner is FrmControleEntregas frmControleDeEntregas)
                {
                    // Preenche os campos no formulário FrmPedido com os dados do produto
                    frmControleDeEntregas.ProdutoID = ProdutoID;
                    frmControleDeEntregas.txtNomeProduto.Text = Nome;
                    frmControleDeEntregas.txtPrecoUnit.Text = Preco.ToString();
                    frmControleDeEntregas.txtQuantidade.Text = "1";

                    // Calcula o subtotal
                    frmControleDeEntregas.CalcularSubtotal();
                }
                // Fecha o formulário FrmLocalizarProduto
                this.Close();
            }
            finally
            {
                // Certifica-se de que a variável isSelectingProduct seja false ao final do processo
                isSelectingProduct = false;
            }
        }
        public void Listar()
        {
            ProdutoBLL objetoBll = new ProdutoBLL();
            dataGridPesquisar.DataSource = objetoBll.Listar();
            PersonalizarDataGridView(dataGridPesquisar);
            txtPesquisa.Focus();
        }
        public new int ObterLinhaAtual()
        {
            return LinhaAtual;
        }
        private void FrmPesquisarProduto_Load(object sender, EventArgs e)
        {
            Listar();
        }

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string nome = "%" + txtPesquisa.Text + "%";
            ProdutoDAL dao = new ProdutoDAL();

            if (rbtCodigo.Checked)
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorCodigo(nome);
            }
            else
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorNome(nome);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridPesquisar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelecionarProduto();
        }

        private void dataGridPesquisar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && dataGridPesquisar.CurrentCell.RowIndex == 0)
            {
                txtPesquisa.Focus();
            }

            if (dataGridPesquisar.CurrentRow != null)
            {
                LinhaAtual = dataGridPesquisar.CurrentRow.Index;
            }
            // Verifica se a tecla pressionada foi a seta para baixo
            if (e.KeyCode == Keys.Down)
            {
                // Move o foco para o DataGridView
                dataGridPesquisar.Focus();

                // Seleciona a primeira linha se houver linhas
                if (dataGridPesquisar.Rows.Count > 0)
                {
                    dataGridPesquisar.ClearSelection();
                    dataGridPesquisar.Rows[0].Selected = true;
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                // Supondo que a seleção está habilitada em FullRowSelect para capturar a linha completa
                var selectedRow = dataGridPesquisar.CurrentRow;
                if (selectedRow != null)
                {
                    this.Close();
                }
            }
        }

        private void dataGridPesquisar_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridPesquisar.CurrentRow != null)
            {
                LinhaAtual = dataGridPesquisar.CurrentRow.Index;
            }
        }

        private void FrmPesquisarProduto_FormClosing(object sender, FormClosingEventArgs e)
        {
            SelecionarProduto();
        }
    }
}
