using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;

namespace ComissPro
{
    public partial class FrmPrestacaoDeContasDataGrid : KryptonForm
    {
        private List<EntregasModel> entregasSelecionadas;
        public string StatusOperacao { get; set; }
        private string QueryPrestacao = "SELECT MAX(PrestacaoID) FROM PrestacaoContas";
        public int PrestacaoID { get; set; }
        private bool linhaTotaisAdicionada = false; // Flag para controlar a adição da linha de totais


        public FrmPrestacaoDeContasDataGrid(string statusOperacao)
        {
            this.StatusOperacao = statusOperacao;
            InitializeComponent();

            // Associar o evento Leave ao txtTotalDevolvido
            txtTotalDevolvida.Leave += txtTotalDevolvida_Leave;

            ConfigurarDataGridView();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }

        private void ConfigurarDataGridView()
        {
            dgvPrestacaoDeContas.AutoGenerateColumns = false;
            dgvPrestacaoDeContas.Columns.Clear();

            var colEntregaID = new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "ID\nEntrega", ReadOnly = true, Visible = false };
            colEntregaID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colEntregaID);

            // "Vendedor" em uma linha só, mas centralizado
            var colVendedor = new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", ReadOnly = true, Width = 170 };
            colVendedor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colVendedor);

            var colProduto = new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = " Produto\nNome", ReadOnly = true, Width = 150, Visible = false };
            colProduto.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colProduto);

            var colQtdEntregue = new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "  Bilhetes\nEntregues", ReadOnly = true, Width = 80 };
            colQtdEntregue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdEntregue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdEntregue);

            var colPrecoUnit = new DataGridViewTextBoxColumn { Name = "PrecoUnit", HeaderText = " Preço\nUnitário", ReadOnly = true, Width = 70 };
            colPrecoUnit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colPrecoUnit.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPrecoUnit);

            var colDataEntrega = new DataGridViewTextBoxColumn { Name = "DataEntrega", HeaderText = "  Data\nEntrega", ReadOnly = true, Width = 80 };
            colDataEntrega.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDataEntrega.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colDataEntrega);

            var colQtdDevolvida = new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = " Bilhetes\nDevolvidos", ReadOnly = false, Width = 75 };
            colQtdDevolvida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdDevolvida.DefaultCellStyle.BackColor = Color.LightBlue;
            colQtdDevolvida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdDevolvida);

            var colQtdVendida = new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = " Bilhetes\nVendidos", ReadOnly = true, Width = 75 };
            colQtdVendida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdVendida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdVendida);

            var colValorRecebido = new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = " Valor\nRecebido", ReadOnly = true, Width = 100 };
            colValorRecebido.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colValorRecebido.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colValorRecebido);

            var colPercentualComissao = new DataGridViewTextBoxColumn { Name = "PercentualComissao", HeaderText = "   %\nComissão", ReadOnly = false, Width = 75 };
            colPercentualComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercentualComissao.DefaultCellStyle.BackColor = Color.LightBlue;
            colPercentualComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPercentualComissao);

            // "Comissão" em uma linha só, mas centralizado
            var colComissao = new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", ReadOnly = true, Width = 100 };
            colComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colComissao);

            var colDataPrestacao = new DataGridViewTextBoxColumn { Name = "DataPrestacao", HeaderText = "  Data\nPrestação", ReadOnly = false, Width = 90 };
            colDataPrestacao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDataPrestacao.DefaultCellStyle.BackColor = Color.LightBlue;
            colDataPrestacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colDataPrestacao);

            // Garantir que os cabeçalhos tenham altura suficiente
            dgvPrestacaoDeContas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void CarregarEntregasNoDataGrid(int vendedorID)
        {
            dgvPrestacaoDeContas.Rows.Clear();
            linhaTotaisAdicionada = false; // Resetar a flag ao carregar novos dados
            entregasSelecionadas = new EntregasDal().CarregarEntregasNaoPrestadas().FindAll(e => e.VendedorID == vendedorID);

            if (entregasSelecionadas.Count == 0)
            {
                MessageBox.Show("Nenhuma entrega pendente encontrada para este vendedor.");
                AtualizarTotaisNosTextBoxes(); // Zerar os TextBoxes
                return;
            }

            foreach (var entrega in entregasSelecionadas)
            {
                dgvPrestacaoDeContas.Rows.Add(
                    entrega.EntregaID,
                    entrega.Nome,
                    entrega.NomeProduto,
                    entrega.QuantidadeEntregue,
                    entrega.Preco.ToString("C"),
                    entrega.DataEntrega.Value.ToString("dd/MM/yyyy"),
                    "0",
                    entrega.QuantidadeEntregue,
                    (entrega.QuantidadeEntregue * entrega.Preco).ToString("C"),
                    entrega.Comissao.ToString("F2"),
                    (entrega.QuantidadeEntregue * entrega.Preco * (entrega.Comissao / 100)).ToString("C"),
                    DateTime.Now.ToString("dd/MM/yyyy")
                );
            }

            // Sempre adicionar a linha de totais, mesmo com uma única entrega
            AdicionarLinhaTotais();
            linhaTotaisAdicionada = true;
            AtualizarTotaisNosTextBoxes();
        }

        private void AdicionarLinhaTotais()
        {
            int totalBilhetesEntregues = 0;
            int totalBilhetesVendidos = 0;
            int totalBilhetesDevolvidos = 0;
            double totalRecebido = 0;
            double totalComissao = 0;

            foreach (var entrega in entregasSelecionadas)
            {
                totalBilhetesEntregues += entrega.QuantidadeEntregue;
            }

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                if (row.Cells["Nome"].Value?.ToString() != "Totais")
                {
                    int qtdVendida = int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0");
                    int qtdDevolvida = int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0");

                    totalBilhetesVendidos += qtdVendida;
                    totalBilhetesDevolvidos += qtdDevolvida;
                    totalRecebido += double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency);
                    totalComissao += double.Parse(row.Cells["Comissao"].Value?.ToString() ?? "0", NumberStyles.Currency);
                }
            }

            int index = dgvPrestacaoDeContas.Rows.Add();
            var rowTotal = dgvPrestacaoDeContas.Rows[index];
            rowTotal.Cells["Nome"].Value = "Totais";
            rowTotal.Cells["QuantidadeEntregue"].Value = totalBilhetesEntregues.ToString();
            rowTotal.Cells["QuantidadeVendida"].Value = totalBilhetesVendidos.ToString();
            rowTotal.Cells["QuantidadeDevolvida"].Value = totalBilhetesDevolvidos.ToString();
            rowTotal.Cells["ValorRecebido"].Value = totalRecebido.ToString("C");
            rowTotal.Cells["Comissao"].Value = totalComissao.ToString("C");
            rowTotal.ReadOnly = true;
            AplicarEstiloLinhaTotais();
        }

        private void AtualizarLinhaTotais()
        {
            int totalBilhetesEntregues = 0;
            int totalBilhetesVendidos = 0;
            int totalBilhetesDevolvidos = 0;
            double totalRecebido = 0;
            double totalComissao = 0;

            foreach (var entrega in entregasSelecionadas)
            {
                totalBilhetesEntregues += entrega.QuantidadeEntregue;
            }

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                if (row.Cells["Nome"].Value?.ToString() != "Totais")
                {
                    int qtdVendida = int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0");
                    int qtdDevolvida = int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0");

                    totalBilhetesVendidos += qtdVendida;
                    totalBilhetesDevolvidos += qtdDevolvida;
                    totalRecebido += double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency);
                    totalComissao += double.Parse(row.Cells["Comissao"].Value?.ToString() ?? "0", NumberStyles.Currency);
                }
            }

            var rowTotal = dgvPrestacaoDeContas.Rows[dgvPrestacaoDeContas.Rows.Count - 1];
            rowTotal.Cells["QuantidadeEntregue"].Value = totalBilhetesEntregues.ToString();
            rowTotal.Cells["QuantidadeVendida"].Value = totalBilhetesVendidos.ToString();
            rowTotal.Cells["QuantidadeDevolvida"].Value = totalBilhetesDevolvidos.ToString();
            rowTotal.Cells["ValorRecebido"].Value = totalRecebido.ToString("C");
            rowTotal.Cells["Comissao"].Value = totalComissao.ToString("C");
            AplicarEstiloLinhaTotais();
        }

        private void AtualizarTotaisNosTextBoxes()
        {
            int totalBilhetesEntregues = 0;
            int totalBilhetesVendidos = 0;
            int totalBilhetesDevolvidos = 0;
            double totalRecebido = 0;
            double totalComissao = 0;

            foreach (var entrega in entregasSelecionadas)
            {
                totalBilhetesEntregues += entrega.QuantidadeEntregue;
            }

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                if (row.Cells["Nome"].Value?.ToString() != "Totais")
                {
                    int qtdVendida = int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0");
                    int qtdDevolvida = int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0");

                    totalBilhetesVendidos += qtdVendida;
                    totalBilhetesDevolvidos += qtdDevolvida;
                    totalRecebido += double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency);
                    totalComissao += double.Parse(row.Cells["Comissao"].Value?.ToString() ?? "0", NumberStyles.Currency);
                }
            }

            txtTotalEntregue.Text = totalBilhetesEntregues.ToString();
            txtTotalVendida.Text = totalBilhetesVendidos.ToString();
            txtTotalDevolvida.Text = totalBilhetesDevolvidos.ToString();
            txtTotalRecebido.Text = totalRecebido.ToString("C");
            txtTotalComissao.Text = totalComissao.ToString("C");
        }

        private void AplicarEstiloLinhaTotais()
        {
            if (dgvPrestacaoDeContas.Rows.Count > 0)
            {
                var rowTotal = dgvPrestacaoDeContas.Rows[dgvPrestacaoDeContas.Rows.Count - 1];
                if (rowTotal.Cells["Nome"].Value?.ToString() == "Totais")
                {
                    rowTotal.DefaultCellStyle.Font = new Font(dgvPrestacaoDeContas.Font, FontStyle.Bold);
                    rowTotal.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }
        private void CarregarComboEntregas()
        {
            try
            {
                var entregas = new EntregasDal().CarregarEntregasNaoPrestadas();
                if (entregas == null || entregas.Count == 0)
                {
                    MessageBox.Show("Nenhuma entrega pendente encontrada.");
                    return;
                }

                cmbEntregasPendentes.DataSource = null;
                cmbEntregasPendentes.Items.Clear();
                cmbEntregasPendentes.DisplayMember = "ToString";
                cmbEntregasPendentes.ValueMember = "EntregaID";
                cmbEntregasPendentes.DataSource = entregas;

                if (entregas.Count > 0)
                {
                    cmbEntregasPendentes.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar entregas: {ex.Message}");
            }
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (dgvPrestacaoDeContas.Rows.Count == 0)
            {
                MessageBox.Show("Nenhuma entrega selecionada para prestar contas!");
                return;
            }

            var prestacaoDAL = new PrestacaoDeContasDAL();
            var fluxoCaixaDAL = new FluxoCaixaDAL();
            int prestacaoID = PrestacaoID;

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                if (row.Cells["Nome"].Value?.ToString() == "Totais") continue;

                var prestacao = new PrestacaoContasModel
                {
                    PrestacaoID = prestacaoID,
                    EntregaID = int.Parse(row.Cells["EntregaID"].Value?.ToString() ?? "0"),
                    QuantidadeVendida = int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0"),
                    QuantidadeDevolvida = int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0"),
                    ValorRecebido = double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency),
                    Comissao = double.Parse(row.Cells["Comissao"].Value?.ToString() ?? "0", NumberStyles.Currency),
                    DataPrestacao = DateTime.Parse(row.Cells["DataPrestacao"].Value?.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy")),
                    Nome = row.Cells["Nome"].Value?.ToString(),
                    VendedorID = entregasSelecionadas.Find(entrega => entrega.EntregaID == int.Parse(row.Cells["EntregaID"].Value.ToString())).VendedorID
                };
                prestacaoDAL.SalvarPrestacaoDeContas(prestacao);

                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "ENTRADA",
                    Valor = prestacao.ValorRecebido,
                    DataMovimentacao = prestacao.DataPrestacao,
                    Descricao = $"Prestação de contas - {prestacao.Nome}",
                    PrestacaoID = prestacaoID
                });

                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "SAIDA",
                    Valor = prestacao.Comissao,
                    DataMovimentacao = prestacao.DataPrestacao,
                    Descricao = $"Comissão paga - {prestacao.Nome}",
                    PrestacaoID = prestacaoID
                });
            }

            MessageBox.Show("Prestações de contas salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CarregarComboEntregas();
            ((FrmManutençãodeEntregaBilhetes)Application.OpenForms["FrmManutençãodeEntregaBilhetes"])?.HabilitarTimer(true);
            Utilitario.LimpaCampoKrypton(this);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        private void FrmPrestacaoDeContasDataGrid_KeyDown(object sender, KeyEventArgs e)
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
                if (MessageBox.Show("Deseja sair?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void FrmPrestacaoDeContasDataGrid_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryPrestacao);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                //string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                PrestacaoID = NovoCodigo;

            }
            CarregarComboEntregas();
        }

        private void dgvPrestacaoDeContas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvPrestacaoDeContas.Rows[e.RowIndex];
            if (row.Cells["Nome"].Value?.ToString() == "Totais") return;

            if (e.ColumnIndex == dgvPrestacaoDeContas.Columns["QuantidadeDevolvida"].Index)
            {
                int entregue = int.Parse(row.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0");
                string devolvidaText = row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0";

                if (!int.TryParse(devolvidaText, out int devolvida) || devolvida < 0)
                {
                    MessageBox.Show("Digite uma quantidade válida e não negativa!");
                    row.Cells["QuantidadeDevolvida"].Value = "0";
                    devolvida = 0;
                }

                if (devolvida > entregue)
                {
                    MessageBox.Show("Quantidade devolvida não pode ser maior que a entregue!");
                    row.Cells["QuantidadeDevolvida"].Value = "0";
                    devolvida = 0;
                }

                int vendida = entregue - devolvida;
                double precoUnit = double.Parse(row.Cells["PrecoUnit"].Value?.ToString() ?? "0", NumberStyles.Currency);
                double percentualComissao = double.Parse(row.Cells["PercentualComissao"].Value?.ToString() ?? "0");
                double valorRecebido = vendida * precoUnit;
                double comissao = valorRecebido * (percentualComissao / 100);

                row.Cells["QuantidadeVendida"].Value = vendida.ToString();
                row.Cells["ValorRecebido"].Value = valorRecebido.ToString("C");
                row.Cells["Comissao"].Value = comissao.ToString("C");
            }
            else if (e.ColumnIndex == dgvPrestacaoDeContas.Columns["PercentualComissao"].Index)
            {
                string percentualText = row.Cells["PercentualComissao"].Value?.ToString() ?? "0";
                if (!double.TryParse(percentualText, out double percentualComissao) || percentualComissao < 0 || percentualComissao > 100)
                {
                    MessageBox.Show("Digite um percentual válido entre 0 e 100!");
                    row.Cells["PercentualComissao"].Value = entregasSelecionadas[e.RowIndex].Comissao.ToString("F2");
                    percentualComissao = entregasSelecionadas[e.RowIndex].Comissao;
                }

                double valorRecebido = double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency);
                double comissao = valorRecebido * (percentualComissao / 100);
                row.Cells["Comissao"].Value = comissao.ToString("C");
            }
            else if (e.ColumnIndex == dgvPrestacaoDeContas.Columns["DataPrestacao"].Index)
            {
                string dataText = row.Cells["DataPrestacao"].Value?.ToString();
                if (!DateTime.TryParse(dataText, out _))
                {
                    MessageBox.Show("Digite uma data válida no formato dd/MM/yyyy!");
                    row.Cells["DataPrestacao"].Value = DateTime.Now.ToString("dd/MM/yyyy");
                }
            }

            if (linhaTotaisAdicionada)
            {
                AtualizarLinhaTotais();
            }
            AtualizarTotaisNosTextBoxes();
        }

        private void txtTotalDevolvida_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(txtTotalDevolvida.Text, out int totalDevolvido) || totalDevolvido < 0)
            {
                MessageBox.Show("Digite uma quantidade válida e não negativa!");
                AtualizarTotaisNosTextBoxes(); // Reverte para o valor anterior
                return;
            }

            int totalEntregue = int.Parse(txtTotalEntregue.Text);
            if (totalDevolvido > totalEntregue)
            {
                MessageBox.Show("Quantidade devolvida não pode ser maior que a entregue!");
                AtualizarTotaisNosTextBoxes(); // Reverte para o valor anterior
                return;
            }

            // Distribuir o total devolvido entre as linhas do grid
            int linhasNormais = dgvPrestacaoDeContas.Rows.Count - (linhaTotaisAdicionada ? 1 : 0);
            if (linhasNormais == 0) return;

            int devolvidoRestante = totalDevolvido;
            for (int i = 0; i < linhasNormais; i++)
            {
                var row = dgvPrestacaoDeContas.Rows[i];
                int entregue = int.Parse(row.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0");
                int devolvidoPorLinha = Math.Min(devolvidoRestante, entregue); // Não ultrapassa o entregue

                if (i == linhasNormais - 1) // Última linha recebe o restante
                {
                    devolvidoPorLinha = devolvidoRestante;
                }

                row.Cells["QuantidadeDevolvida"].Value = devolvidoPorLinha.ToString();
                int vendida = entregue - devolvidoPorLinha;
                double precoUnit = double.Parse(row.Cells["PrecoUnit"].Value?.ToString() ?? "0", NumberStyles.Currency);
                double percentualComissao = double.Parse(row.Cells["PercentualComissao"].Value?.ToString() ?? "0");
                double valorRecebido = vendida * precoUnit;
                double comissao = valorRecebido * (percentualComissao / 100);

                row.Cells["QuantidadeVendida"].Value = vendida.ToString();
                row.Cells["ValorRecebido"].Value = valorRecebido.ToString("C");
                row.Cells["Comissao"].Value = comissao.ToString("C");

                devolvidoRestante -= devolvidoPorLinha;
                if (devolvidoRestante <= 0) break;
            }

            // Atualizar totais após a distribuição
            AtualizarLinhaTotais();
            AtualizarTotaisNosTextBoxes();
        }

        private void FrmPrestacaoDeContasDataGrid_KeyDown_1(object sender, KeyEventArgs e)
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

        private void cmbEntregasPendentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEntregasPendentes.SelectedItem != null)
            {
                var entregaSelecionada = (EntregasModel)cmbEntregasPendentes.SelectedItem;
                CarregarEntregasNoDataGrid(entregaSelecionada.VendedorID);
                lblPercentualComissao.Text = entregaSelecionada.Comissao.ToString("F2") + "%";
            }
        }
    }


    public class EntregasModel
    {
        public int EntregaID { get; set; }
        public int VendedorID { get; set; }
        public int ProdutoID { get; set; }
        public int QuantidadeEntregue { get; set; }

        public int QuantidadeVendida { get; set; }
        public int QuantidadeDevolvida { get; set; }
        public double ValorRecebido { get; set; }

        public DateTime? DataEntrega { get; set; }
        public double Preco { get; set; }
        public double Comissao { get; set; }
        public string Nome { get; set; }
        public string NomeProduto { get; set; }
        public double Total { get; set; }

        public override string ToString()
        {
            return $"{Nome} - Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
        }
    }
}
