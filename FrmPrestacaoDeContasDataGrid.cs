using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;
using System.Linq;

namespace ComissPro
{
    public partial class FrmPrestacaoDeContasDataGrid : KryptonForm
    {
        private List<EntregasModel> entregasSelecionadas;
        public string StatusOperacao { get; set; }
        private string QueryPrestacao = "SELECT MAX(PrestacaoID) FROM PrestacaoContas";
        public int PrestacaoID { get; set; }
        private bool linhaTotaisAdicionada = false;
        private bool bloqueiaPesquisa = false;
        public bool bloqueiaEventosTextChanged = false;

        private int? vendedorIDInicial; // Armazena o VendedorID passado, se houver
        public FrmPrestacaoDeContasDataGrid(string statusOperacao, int? vendedorID = null)
        {
            
            InitializeComponent();
            this.StatusOperacao = statusOperacao;
            this.vendedorIDInicial = vendedorID; // Armazena o VendedorID passado   
            txtTotalDevolvida.Leave += txtTotalDevolvida_Leave;
            ConfigurarDataGridView();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        private void FrmPrestacaoDeContasDataGrid_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryPrestacao);
                PrestacaoID = NovoCodigo;
                SelecionarEntregaPendente(); // Chama o método ajustado
            }
            dgvPrestacaoDeContas.CellValueChanged += dgvPrestacaoDeContas_CellValueChanged;
        }

        private void SelecionarEntregaPendente()
        {
            try
            {
                if (vendedorIDInicial.HasValue)
                {
                    // Se um VendedorID foi passado pelo construtor, usa diretamente
                    CarregarEntregasNoDataGrid(vendedorIDInicial.Value);
                }
                else
                {
                    // Caso contrário, abre o FrmSelecionarEntrega como antes
                    using (FrmSelecionarEntrega frmSelecionar = new FrmSelecionarEntrega(this, txtLocalizarVendedor.Text))
                    {
                        if (frmSelecionar.ShowDialog() == DialogResult.OK && frmSelecionar.VendedorSelecionadoID.HasValue)
                        {
                            int vendedorID = frmSelecionar.VendedorSelecionadoID.Value;
                            CarregarEntregasNoDataGrid(vendedorID);
                        }
                        else
                        {
                            MessageBox.Show("Nenhum vendedor selecionado. Selecione um vendedor para continuar.",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar vendedor: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarEntregasNoDataGrid(int vendedorID)
        {
            try
            {
                dgvPrestacaoDeContas.Rows.Clear();
                linhaTotaisAdicionada = false;
                entregasSelecionadas = new EntregasDal().CarregarEntregasNaoPrestadas().FindAll(e => e.VendedorID == vendedorID);

                if (entregasSelecionadas.Count == 0)
                {
                    MessageBox.Show("Nenhuma entrega pendente encontrada para este vendedor.", "Informação!",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    AtualizarTotaisNosTextBoxes();
                    return;
                }

                foreach (var entrega in entregasSelecionadas)
                {
                    dgvPrestacaoDeContas.Rows.Add(
                        false, // Checkbox inicialmente desmarcado
                        entrega.EntregaID,
                        entrega.Nome,
                        entrega.NomeProduto,
                        entrega.QuantidadeEntregue,
                        entrega.Preco.ToString("C"),
                        entrega.DataEntrega.Value.ToString("dd/MM/yyyy"),
                        "0", // QuantidadeDevolvida
                        entrega.QuantidadeEntregue, // QuantidadeVendida inicial
                        (entrega.QuantidadeEntregue * entrega.Preco).ToString("C"), // ValorRecebido
                        entrega.Comissao.ToString("F2"), // PercentualComissao inicial
                        (entrega.QuantidadeEntregue * entrega.Preco * (entrega.Comissao / 100)).ToString("C"), // Comissao
                        DateTime.Now.ToString("dd/MM/yyyy") // DataPrestacao
                    );
                }

                AdicionarLinhaTotais();
                linhaTotaisAdicionada = true;
                AtualizarTotaisNosTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar entregas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvPrestacaoDeContas.AutoGenerateColumns = false;
            dgvPrestacaoDeContas.Columns.Clear();

            // Adicionar coluna de Checkbox
            var colCheckbox = new DataGridViewCheckBoxColumn
            {
                Name = "Selecionado",
                HeaderText = "Sel.",
                Width = 50,
                ReadOnly = false,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            };
            dgvPrestacaoDeContas.Columns.Add(colCheckbox);           
            // Adicionar colunas Acima do DataGridView em 06/03/2025

            var colEntregaID = new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "ID\nEntrega", ReadOnly = true, Visible = false };
            colEntregaID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colEntregaID);

            var colVendedor = new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", ReadOnly = true, Width = 170 };
            colVendedor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colVendedor);

            var colProduto = new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = "Produto\nNome", ReadOnly = true, Width = 150, Visible = false };
            colProduto.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colProduto);

            var colQtdEntregue = new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "Bilhetes\nEntregues", ReadOnly = true, Width = 80 };
            colQtdEntregue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdEntregue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdEntregue);

            var colPrecoUnit = new DataGridViewTextBoxColumn { Name = "PrecoUnit", HeaderText = "Preço\nUnitário", ReadOnly = true, Width = 70 };
            colPrecoUnit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colPrecoUnit.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPrecoUnit);

            var colDataEntrega = new DataGridViewTextBoxColumn { Name = "DataEntrega", HeaderText = "Data\nEntrega", ReadOnly = true, Width = 80 };
            colDataEntrega.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDataEntrega.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colDataEntrega);

            var colQtdDevolvida = new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = "Bilhetes\nDevolvidos", ReadOnly = false, Width = 75 };
            colQtdDevolvida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdDevolvida.DefaultCellStyle.BackColor = Color.Orange;
            colQtdDevolvida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
           
            //Implementado depois por Wadson           
            //colQtdDevolvida.HeaderCell.Style.BackColor = Color.DarkBlue; // Cor de fundo
            //colQtdDevolvida.HeaderCell.Style.ForeColor = Color.White; // Cor do texto
            //colQtdDevolvida.HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold); // Fonte personalizada
            //Fim da implementação de Wadson
            dgvPrestacaoDeContas.Columns.Add(colQtdDevolvida);

            var colQtdVendida = new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = "Bilhetes\nVendidos", ReadOnly = true, Width = 75 };
            colQtdVendida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdVendida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdVendida);

            var colValorRecebido = new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = "Valor\nRecebido", ReadOnly = true, Width = 100 };
            colValorRecebido.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colValorRecebido.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colValorRecebido);

            var colPercentualComissao = new DataGridViewTextBoxColumn { Name = "PercentualComissao", HeaderText = "%\nComissão", ReadOnly = false, Width = 75 };
            colPercentualComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercentualComissao.DefaultCellStyle.BackColor = Color.LightBlue;
            colPercentualComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPercentualComissao);

            var colComissao = new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", ReadOnly = true, Width = 100 };
            colComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colComissao);

            var colDataPrestacao = new DataGridViewTextBoxColumn { Name = "DataPrestacao", HeaderText = "Data\nPrestação", ReadOnly = false, Width = 90 };
            colDataPrestacao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDataPrestacao.DefaultCellStyle.BackColor = Color.LightBlue;
            colDataPrestacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colDataPrestacao);

            dgvPrestacaoDeContas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
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
            rowTotal.Cells["Selecionado"].Value = null; // Sem checkbox na linha de totais
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

        private void AtualizarTotaisNosTextBoxes(bool apenasMarcadas = false)
        {
            int totalBilhetesEntregues = 0;
            int totalBilhetesVendidos = 0;
            int totalBilhetesDevolvidos = 0;
            double totalRecebido = 0;
            double totalComissao = 0;

            var linhas = apenasMarcadas
                ? dgvPrestacaoDeContas.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["Selecionado"].Value != null && (bool)row.Cells["Selecionado"].Value)
                : dgvPrestacaoDeContas.Rows.Cast<DataGridViewRow>();

            foreach (DataGridViewRow row in linhas)
            {
                if (row.Cells["Nome"].Value?.ToString() != "Totais")
                {
                    totalBilhetesEntregues += int.Parse(row.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0");
                    totalBilhetesVendidos += int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0");
                    totalBilhetesDevolvidos += int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0");
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


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            var linhasMarcadas = dgvPrestacaoDeContas.Rows.Cast<DataGridViewRow>()
     .Where(row => row.Cells["Selecionado"].Value != null && (bool)row.Cells["Selecionado"].Value
                && row.Cells["Nome"].Value?.ToString() != "Totais")
     .ToList();

            if (linhasMarcadas.Count == 0)
            {
                MessageBox.Show("Nenhuma entrega marcada para prestar contas! Marque pelo menos uma linha.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var prestacaoDAL = new PrestacaoDeContasDAL();
            var fluxoCaixaDAL = new FluxoCaixaDAL();
            var entregasRemover = new List<int>(); // Lista para armazenar os EntregaID a remover

            // Salvar as prestações e coletar os IDs a remover
            foreach (DataGridViewRow row in linhasMarcadas)
            {
                var entregaID = int.Parse(row.Cells["EntregaID"].Value?.ToString() ?? "0");
                var prestacao = new PrestacaoContasModel
                {
                    EntregaID = entregaID,
                    QuantidadeVendida = int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0"),
                    QuantidadeDevolvida = int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0"),
                    ValorRecebido = double.Parse(row.Cells["ValorRecebido"].Value?.ToString().Replace("R$", "").Trim() ?? "0", NumberStyles.Currency),
                    Comissao = double.Parse(row.Cells["Comissao"].Value?.ToString().Replace("R$", "").Trim() ?? "0", NumberStyles.Currency),
                    DataPrestacao = DateTime.Parse(row.Cells["DataPrestacao"].Value?.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy")),
                    Nome = row.Cells["Nome"].Value?.ToString(),
                    VendedorID = entregasSelecionadas.Find(entrega => entrega.EntregaID == entregaID).VendedorID
                };

                // Salvar a prestação e obter o PrestacaoID gerado
                prestacaoDAL.SalvarPrestacaoDeContas(prestacao);

                // Registrar movimentações no fluxo de caixa
                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "ENTRADA",
                    Valor = prestacao.ValorRecebido,
                    DataMovimentacao = prestacao.DataPrestacao,
                    Descricao = $"Prestação de contas - {prestacao.Nome}",
                    PrestacaoID = prestacao.PrestacaoID
                });

                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "SAÍDA",
                    Valor = prestacao.Comissao,
                    DataMovimentacao = prestacao.DataPrestacao,
                    Descricao = $"Comissão paga - {prestacao.Nome}",
                    PrestacaoID = prestacao.PrestacaoID
                });

                entregasRemover.Add(entregaID); // Adicionar o EntregaID à lista de remoção
            }

            MessageBox.Show("Prestações de contas salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ((FrmManutencaodeEntregaBilhetes)Application.OpenForms["FrmManutencaodeEntregaBilhetes"])?.HabilitarTimer(true);

            // Remover as linhas do DataGridView e da lista de entregas usando os IDs coletados
            foreach (var entregaID in entregasRemover)
            {
                var rowToRemove = dgvPrestacaoDeContas.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["EntregaID"].Value != null && int.Parse(row.Cells["EntregaID"].Value.ToString()) == entregaID);
                if (rowToRemove != null)
                {
                    dgvPrestacaoDeContas.Rows.Remove(rowToRemove);
                }
                entregasSelecionadas.RemoveAll(entrega => entrega.EntregaID == entregaID);
            }

            // Atualizar a linha de totais e os TextBox
            if (dgvPrestacaoDeContas.Rows.Count > 1) // Se ainda houver linhas além da "Totais"
            {
                AtualizarLinhaTotais();
                AtualizarTotaisNosTextBoxes();
            }
            else
            {
                dgvPrestacaoDeContas.Rows.Clear();
                linhaTotaisAdicionada = false;
                txtTotalEntregue.Text = "0";
                txtTotalVendida.Text = "0";
                txtTotalDevolvida.Text = "0";
                txtTotalRecebido.Text = 0.ToString("C");
                txtTotalComissao.Text = 0.ToString("C");
            }

            Utilitario.LimpaCampoKrypton(this);
            txtLocalizarVendedor.Focus();
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
                AtualizarTotaisNosTextBoxes();
                return;
            }

            int totalEntregue = int.Parse(txtTotalEntregue.Text);
            if (totalDevolvido > totalEntregue)
            {
                MessageBox.Show("Quantidade devolvida não pode ser maior que a entregue!");
                AtualizarTotaisNosTextBoxes();
                return;
            }

            int linhasNormais = dgvPrestacaoDeContas.Rows.Count - (linhaTotaisAdicionada ? 1 : 0);
            if (linhasNormais == 0) return;

            int devolvidoRestante = totalDevolvido;
            for (int i = 0; i < linhasNormais; i++)
            {
                var row = dgvPrestacaoDeContas.Rows[i];
                int entregue = int.Parse(row.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0");
                int devolvidoPorLinha = Math.Min(devolvidoRestante, entregue);

                if (i == linhasNormais - 1)
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

        private void btnLocalizarVendedor_Click(object sender, EventArgs e)
        {
        }

        private void txtLocalizarVendedor_TextChanged(object sender, EventArgs e)
        {
            if (bloqueiaPesquisa || bloqueiaEventosTextChanged || string.IsNullOrEmpty(txtLocalizarVendedor.Text))
                return;

            bloqueiaPesquisa = true;

            using (FrmSelecionarEntrega pesquisaVendedor = new FrmSelecionarEntrega(this, txtLocalizarVendedor.Text))
            {
                pesquisaVendedor.Owner = this;
                if (pesquisaVendedor.ShowDialog() == DialogResult.OK && pesquisaVendedor.VendedorSelecionadoID.HasValue)
                {
                    CarregarEntregasNoDataGrid(pesquisaVendedor.VendedorSelecionadoID.Value);
                }
            }

            Task.Delay(100).ContinueWith(t =>
            {
                Invoke(new Action(() => bloqueiaPesquisa = false));
            });
        }

        private void dgvPrestacaoDeContas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvPrestacaoDeContas.Columns["Selecionado"].Index)
            {
                AtualizarTotaisNosTextBoxes(true); // Atualizar apenas com base nas linhas marcadas
            }
            else
            {
                AtualizarTotaisNosTextBoxes(); // Atualizar normalmente
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
        public bool Prestacaorealizada { get; set; }

        public override string ToString()
        {
            return $"{Nome} - Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
        }
    }
}
