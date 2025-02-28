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
    public partial class FrmLocalicarVendedor : ComissPro.FrmModelo
    {
        protected int LinhaAtual = -1;
        public int VendedorID { get; set; }
        public string Nome { get; set; }       
        
        public string vendedorSelecionado { get; set; }
        public Form FormChamador { get; set; }
        public FrmLocalicarVendedor(Form formChamador, string textoDigitado)
        {
            InitializeComponent();

            Utilitario.AdicionarEfeitoFocoEmTodos(this);
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
        private bool isSelectingVendedor = false;
        private Form formChamador;
        public new int ObterLinhaAtual()
        {
            return LinhaAtual;
        }
        private void SelecionarVendedor()
        {
            // Verifica se o processo de seleção de produto já está em andamento
            if (isSelectingVendedor) return;
            isSelectingVendedor = true;

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
                    dataGridPesquisar["VendedorID", LinhaAtual]?.Value == null )
                {
                    // Caso os valores não sejam válidos, exibe uma mensagem de erro
                    MessageBox.Show("Dados do produto inválidos.");
                    return;
                }

                // Converte o valor da célula NomeProduto para string
                VendedorID = int.Parse(dataGridPesquisar["VendedorID", LinhaAtual].Value.ToString());
                Nome = dataGridPesquisar["Nome", LinhaAtual].Value.ToString();                

                // Acrescenta zeros à esquerda do ProdutoID
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(VendedorID, 4);

                // Obtém a instância do formulário FrmPedido (ou usa uma existente)
                if (this.Owner is FrmControleEntregas frmControleDeEntregas)
                {
                    // Preenche os campos no formulário FrmPedido com os dados do produto
                    frmControleDeEntregas.VendedorID = VendedorID;
                    frmControleDeEntregas.txtNomeVendedor.Text = Nome;  
                }
                // Fecha o formulário FrmLocalizarProduto
                this.Close();
            }
            finally
            {
                // Certifica-se de que a variável isSelectingProduct seja false ao final do processo
                isSelectingVendedor = false;
            }
        }
        private void FrmLocalicarVendedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SelecionarVendedor();
        }

        private void dataGridPesquisar_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridPesquisar.CurrentRow != null)
            {
                LinhaAtual = dataGridPesquisar.CurrentRow.Index;
            }
        }
        public void PersonalizarDataGridView(KryptonDataGridView dgv)
        {
            // Renomear colunas
            dgv.Columns[0].Name = "VendedorID";
            dgv.Columns[1].Name = "Nome";
            dgv.Columns[2].Name = "Cpf";
            dgv.Columns[3].Name = "Telefone";

            // Ajustar colunas automaticamente
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Tornar o grid somente leitura
            dgv.ReadOnly = true;

            // Centralizar colunas de IDs e Quantidade
            dgv.Columns["VendedorID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
                        

            // Ocultar a coluna ProdutoID
            dgv.Columns["VendedorID"].Visible = false;

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
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Name == "Nome")
                    column.Width = 250; // Largura maior para a coluna "Nome"
                else
                    column.Width = 100; // Largura padrão para as demais colunas
            }

        }
        public void Listar()
        {
            VendedorBLL objetoBll = new VendedorBLL();
            dataGridPesquisar.DataSource = objetoBll.Listar();
            PersonalizarDataGridView(dataGridPesquisar);
            txtPesquisa.Focus();
        }
        private void FrmLocalicarVendedor_Load(object sender, EventArgs e)
        {
            Listar();
        }

        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.ToLower();
            string nome = "%" + txtPesquisa.Text + "%";
            VendedorDAL dao = new VendedorDAL();

            if (rbtCodigo.Checked)
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorCodigo(nome);
            }
            else
            {
                dataGridPesquisar.DataSource = dao.PesquisarPorNome(nome);
            }
        }

        private void dataGridPesquisar_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelecionarVendedor();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            SelecionarVendedor();
        }

        private void dataGridPesquisar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelecionarVendedor();
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
                    SelecionarVendedor();
                }
            }
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
    }
}
