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
using static ComissPro.Utilitario;

namespace ComissPro
{
    public partial class FrmManipularPrestacaoDeContas : KryptonForm
    {
        private List<EntregasModel> entregasSelecionadas;
        public int VendedorID { get; set; }
        private bool linhaTotaisAdicionada = false;

        public FrmManipularPrestacaoDeContas()
        {
            InitializeComponent();
            ConfigurarDataGridView();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        // Construtor com vendedorID (usado para estorno)
        public FrmManipularPrestacaoDeContas(int vendedorID) : this()
        {
            VendedorID = vendedorID;
        }
        private void FrmPrestacaoDeContasDataGrid_Load(object sender, EventArgs e)
        {
            LogUtil.WriteLog($"FrmManipularPrestacaoDeContas_Load iniciado com VendedorID: {VendedorID}");
            if (VendedorID == 0)
            {
                MessageBox.Show("Nenhum vendedor especificado para carregar prestações.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            CarregarPrestacaoExistente(VendedorID);
        }


        private void ConfigurarDataGridView()
        {
            dgvPrestacaoDeContas.AutoGenerateColumns = false;
            dgvPrestacaoDeContas.Columns.Clear();

            // Coluna CheckBox para marcar estorno
            var colEstornar = new DataGridViewCheckBoxColumn
            {
                Name = "Estornar",
                HeaderText = "Estornar",
                Width = 60,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colEstornar.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colEstornar);

            // Colunas existentes (somente leitura)
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "ID\nPrestação", ReadOnly = true, Visible = false });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "ID\nEntrega", ReadOnly = true, Visible = false });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", ReadOnly = true, Width = 170 });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = "Produto\nNome", ReadOnly = true, Width = 150 });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "Bilhetes\nEntregues", ReadOnly = true, Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrecoUnit", HeaderText = "Preço\nUnitário", ReadOnly = true, Width = 70, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataEntrega", HeaderText = "Data\nEntrega", ReadOnly = true, Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = "Bilhetes\nDevolvidos", ReadOnly = true, Width = 75, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = "Bilhetes\nVendidos", ReadOnly = true, Width = 75, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = "Valor\nRecebido", ReadOnly = true, Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PercentualComissao", HeaderText = "%\nComissão", ReadOnly = true, Width = 75, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", ReadOnly = true, Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvPrestacaoDeContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataPrestacao", HeaderText = "Data\nPrestação", ReadOnly = true, Width = 90, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });

            foreach (DataGridViewColumn col in dgvPrestacaoDeContas.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dgvPrestacaoDeContas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPrestacaoDeContas.AllowUserToOrderColumns = false;
            dgvPrestacaoDeContas.ReadOnly = false;            
        }

        private void CarregarPrestacaoExistente(int vendedorID)
        {
            LogUtil.WriteLog($"Iniciando CarregarPrestacaoExistente com VendedorID: {vendedorID}");
            try
            {
                var prestacaoDAL = new PrestacaoDeContasDAL();
                var entregasDAL = new EntregasDal();

                var prestacoes = prestacaoDAL.CarregarPrestacoesPorVendedorID(vendedorID);
                if (prestacoes.Count == 0)
                {
                    LogUtil.WriteLog("Nenhuma prestação encontrada para este vendedor.");
                    MessageBox.Show("Nenhuma prestação encontrada para este vendedor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                LogUtil.WriteLog($"Prestações carregadas: {prestacoes.Count}");
                dgvPrestacaoDeContas.Rows.Clear();
                entregasSelecionadas = new List<EntregasModel>();

                foreach (var prestacao in prestacoes)
                {
                    var entrega = entregasDAL.CarregarEntregasPorID(prestacao.EntregaID);
                    if (entrega != null)
                    {
                        entregasSelecionadas.Add(entrega);
                        double percentualComissao = prestacao.ValorRecebido != 0 ? (prestacao.Comissao / prestacao.ValorRecebido * 100) : 0;

                        dgvPrestacaoDeContas.Rows.Add(
                            false, // Coluna Estornar inicia desmarcada
                            prestacao.PrestacaoID,
                            entrega.EntregaID,
                            entrega.Nome,
                            entrega.NomeProduto,
                            entrega.QuantidadeEntregue,
                            entrega.Preco.ToString("C"),
                            entrega.DataEntrega.HasValue ? entrega.DataEntrega.Value.ToString("dd/MM/yyyy") : "",
                            prestacao.QuantidadeDevolvida.ToString(),
                            prestacao.QuantidadeVendida.ToString(),
                            prestacao.ValorRecebido.ToString("C"),
                            percentualComissao.ToString("F2"),
                            prestacao.Comissao.ToString("C"),
                            prestacao.DataPrestacao.ToString("dd/MM/yyyy")
                        );
                        LogUtil.WriteLog($"Linha adicionada: EntregaID={entrega.EntregaID}, Nome={entrega.Nome}");
                    }
                }

                AdicionarLinhaTotais();
                linhaTotaisAdicionada = true;
                AtualizarTotaisNosTextBoxes();
                LogUtil.WriteLog("CarregarPrestacaoExistente concluído com sucesso.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar prestação: {ex.Message}");
                MessageBox.Show($"Erro ao carregar prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void AdicionarLinhaTotais()
        {
            int totalBilhetesEntregues = 0, totalBilhetesVendidos = 0, totalBilhetesDevolvidos = 0;
            double totalRecebido = 0, totalComissao = 0;

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                totalBilhetesEntregues += int.Parse(row.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0");
                totalBilhetesVendidos += int.Parse(row.Cells["QuantidadeVendida"].Value?.ToString() ?? "0");
                totalBilhetesDevolvidos += int.Parse(row.Cells["QuantidadeDevolvida"].Value?.ToString() ?? "0");
                totalRecebido += double.Parse(row.Cells["ValorRecebido"].Value?.ToString() ?? "0", NumberStyles.Currency);
                totalComissao += double.Parse(row.Cells["Comissao"].Value?.ToString() ?? "0", NumberStyles.Currency);
            }

            int index = dgvPrestacaoDeContas.Rows.Add();
            var rowTotal = dgvPrestacaoDeContas.Rows[index];
            rowTotal.Cells["Estornar"].Value = false;
            rowTotal.Cells["Nome"].Value = "Totais";
            rowTotal.Cells["QuantidadeEntregue"].Value = totalBilhetesEntregues.ToString();
            rowTotal.Cells["QuantidadeVendida"].Value = totalBilhetesVendidos.ToString();
            rowTotal.Cells["QuantidadeDevolvida"].Value = totalBilhetesDevolvidos.ToString();
            rowTotal.Cells["ValorRecebido"].Value = totalRecebido.ToString("C");
            rowTotal.Cells["Comissao"].Value = totalComissao.ToString("C");
            rowTotal.DefaultCellStyle.Font = new Font(dgvPrestacaoDeContas.Font, FontStyle.Bold);
            rowTotal.DefaultCellStyle.BackColor = Color.LightGray;
            rowTotal.ReadOnly = true;
        }

        private void AtualizarTotaisNosTextBoxes()
        {
            int totalBilhetesEntregues = 0, totalBilhetesVendidos = 0, totalBilhetesDevolvidos = 0;
            double totalRecebido = 0, totalComissao = 0;

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
            {
                if (row.Cells["Nome"].Value?.ToString() != "Totais" &&
                    row.Cells["Estornar"].Value != null &&
                    Convert.ToBoolean(row.Cells["Estornar"].Value))
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

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAlterarOuExcluir_Click(object sender, EventArgs e)
        {
            LogUtil.WriteLog("Iniciando btnAlterarOuExcluir_Click (Estorno)...");
            try
            {
                var prestacoesMarcadas = new List<(int PrestacaoID, int EntregaID, string NomeVendedor, DateTime? DataPrestacao)>();
                var prestacaoDAL = new PrestacaoDeContasDAL();

                // Coletar prestações marcadas
                foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
                {
                    if (row.Cells["Nome"].Value?.ToString() != "Totais" &&
                        row.Cells["Estornar"].Value != null &&
                        Convert.ToBoolean(row.Cells["Estornar"].Value))
                    {
                        int prestacaoID = Convert.ToInt32(row.Cells["PrestacaoID"].Value);
                        int entregaID = Convert.ToInt32(row.Cells["EntregaID"].Value);
                        string nomeVendedor = row.Cells["Nome"].Value.ToString();

                        // Correção para evitar o erro do ternário
                        DateTime? dataPrestacao = null;
                        if (DateTime.TryParse(row.Cells["DataPrestacao"].Value?.ToString(), out DateTime dt))
                        {
                            dataPrestacao = dt;
                        }

                        // Verificar data no banco se necessário
                        if (!dataPrestacao.HasValue)
                        {
                            LogUtil.WriteLog($"DataPrestacao não encontrada no grid para PrestacaoID={prestacaoID}, consultando banco...");
                            dataPrestacao = prestacaoDAL.ObterDataPrestacaoPorID(prestacaoID);
                        }

                        prestacoesMarcadas.Add((prestacaoID, entregaID, nomeVendedor, dataPrestacao));
                    }
                }

                if (prestacoesMarcadas.Count == 0)
                {
                    LogUtil.WriteLog("Nenhuma prestação marcada para estorno.");
                    MessageBox.Show("Marque pelo menos uma prestação para estornar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirmar estorno
                string mensagem = "Deseja estornar as seguintes prestações?\n" +
                    string.Join("\n", prestacoesMarcadas.Select(p => $"- Entrega {p.EntregaID} - {p.NomeVendedor}")) +
                    "\nIsso reverterá as entregas para pendentes e removerá as movimentações financeiras associadas.";

                if (MessageBox.Show(mensagem, "Confirmação de Estorno", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    LogUtil.WriteLog("Estorno cancelado pelo usuário.");
                    return;
                }

                // Processar estorno
                var entregasDAL = new EntregasDal();
                var fluxoCaixaDAL = new FluxoCaixaDAL();
                var linhasParaRemover = new List<int>();

                foreach (var prestacao in prestacoesMarcadas)
                {
                    if (!prestacao.DataPrestacao.HasValue || prestacao.DataPrestacao.Value.Date != DateTime.Today)
                    {
                        LogUtil.WriteLog($"Estorno bloqueado: PrestacaoID={prestacao.PrestacaoID}, DataPrestacao={prestacao.DataPrestacao?.ToString("yyyy-MM-dd") ?? "Nulo"} diferente de hoje.");
                        MessageBox.Show($"A prestação da entrega {prestacao.EntregaID} ({prestacao.NomeVendedor}) não pode ser estornada pois não é do dia atual.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    LogUtil.WriteLog($"Estornando PrestacaoID={prestacao.PrestacaoID}, EntregaID={prestacao.EntregaID}");
                    prestacaoDAL.ExcluirPrestacaoPorEntregaID(prestacao.EntregaID);
                    entregasDAL.MarcarEntregaComoPendente(prestacao.EntregaID);
                    fluxoCaixaDAL.ExcluirMovimentacoesPorPrestacao(prestacao.PrestacaoID);
                    LogUtil.WriteLog($"Estorno concluído para EntregaID={prestacao.EntregaID}");

                    // Marcar linha para remoção
                    int rowIndex = dgvPrestacaoDeContas.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => Convert.ToInt32(r.Cells["EntregaID"].Value) == prestacao.EntregaID)
                        .Select(r => r.Index)
                        .First();
                    linhasParaRemover.Add(rowIndex);
                }

                // Remover linhas estornadas
                foreach (int index in linhasParaRemover.OrderByDescending(i => i))
                {
                    dgvPrestacaoDeContas.Rows.RemoveAt(index);
                }
                // No final do btnAlterarOuExcluir_Click, após remover as linhas
                foreach (int index in linhasParaRemover.OrderByDescending(i => i))
                {
                    dgvPrestacaoDeContas.Rows.RemoveAt(index);
                }

                MessageBox.Show("Prestações marcadas foram estornadas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AtualizarTotaisNosTextBoxes(); // Já está presente, mas confirme               
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao estornar prestações: {ex.Message}");
                MessageBox.Show($"Erro ao estornar prestações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
       

        private void FrmManipularPrestacaoDeContas_KeyDown(object sender, KeyEventArgs e)
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

        private void dgvPrestacaoDeContas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se a célula clicada pertence à coluna de checkbox
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                dgvPrestacaoDeContas.CommitEdit(DataGridViewDataErrorContexts.Commit); // Garante que o valor do checkbox seja atualizado
                AtualizarTotaisNosTextBoxes();
            }
        }
    }
}
