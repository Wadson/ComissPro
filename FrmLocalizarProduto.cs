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
    public partial class FrmLocalizarProduto : KryptonForm
    {
        protected int LinhaAtual = -1;
        public int ProdutoID { get; set; }
        public string Nome { get; set; }
        private decimal Preco;
        private string Unidade;        
        public string produtoSelecionado { get; set; }
        public Form FormChamador { get; set; }
        public FrmLocalizarProduto(Form formChamador, string textoDigitado)
        {
            InitializeComponent();
            // Verifica se o formulário chamador é válido
            if (formChamador != null)
            {
                this.FormChamador = formChamador;
            }
            this.Owner = formChamador;

            txtPesquisa.Text = textoDigitado; // Define a letra pressionada no campo de pesquisa
            txtPesquisa.SelectionStart = txtPesquisa.Text.Length; // Coloca o cursor no final
            txtPesquisa.Focus(); // Foca o campo para continuar digitando

            // Configurar o TextBox para capturar o evento KeyDown
            this.txtPesquisa.KeyDown += new KeyEventHandler(dataGridPesquisar_KeyDown);
            this.dataGridPesquisar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridPesquisar_KeyDown);
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
            dgv.Columns[3].Name = "Unidade";

            // Remover o AutoSizeColumnsMode para permitir larguras fixas
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Ou ajuste conforme necessário

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
            dgv.RowHeadersWidth = 20; // Aumentei de 10 para 20 para mais visibilidade

            // Ajustar largura das colunas individualmente
            dgv.Columns["Nome"].Width = 300;     // Aumentar para 200 pixels (exemplo)
            dgv.Columns["Preco"].Width = 120;    // Aumentar para 150 pixels (exemplo)
            dgv.Columns["Unidade"].Width = 110;  // Aumentar para 120 pixels (exemplo)

            // Configurar cabeçalhos das colunas
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Centralizar cabeçalho da coluna
                column.HeaderCell.Style.WrapMode = DataGridViewTriState.False; // Evitar quebra de texto no cabeçalho
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
                Unidade = dataGridPesquisar["Unidade", LinhaAtual].Value.ToString();                

                // Acrescenta zeros à esquerda do ProdutoID
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(ProdutoID, 4);

                // Obtém a instância do formulário FrmPedido (ou usa uma existente)
                if (this.Owner is FrmCadastroDeEntregas frmControleDeEntregas)
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
            string textoPesquisa = txtPesquisa.Text.ToLower();
            string nome = "%" + txtPesquisa.Text + "%";
            ProdutoDAL dao = new ProdutoDAL();

            dataGridPesquisar.DataSource = dao.PesquisarPorNome(nome);
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
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita o "beep" do Enter no DataGridView

                if (dataGridPesquisar.CurrentRow != null)
                {
                    LinhaAtual = dataGridPesquisar.CurrentRow.Index; // Atualiza a linha atual corretamente
                    SelecionarProduto();
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

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                dataGridPesquisar.Focus();
                if (dataGridPesquisar.Rows.Count > 0)
                {
                    // Verificar se a célula é visível antes de defini-la como CurrentCell
                    DataGridViewCell primeiraCelulaVisivel = null;

                    foreach (DataGridViewCell celula in dataGridPesquisar.Rows[0].Cells)
                    {
                        if (celula.Visible)
                        {
                            primeiraCelulaVisivel = celula;
                            break;
                        }
                    }

                    if (primeiraCelulaVisivel != null)
                    {
                        dataGridPesquisar.CurrentCell = primeiraCelulaVisivel;
                    }
                }
            }
        }

        private void dataGridPesquisar_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelecionarProduto();
        }
    }
}
