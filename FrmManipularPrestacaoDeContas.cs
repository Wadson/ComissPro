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
        public string StatusOperacao { get; set; }
        private string QueryPrestacao = "SELECT MAX(PrestacaoID) FROM PrestacaoContas";
        public int PrestacaoID { get; set; }
        public int VendedorID { get; set; }
        private bool linhaTotaisAdicionada = false;
        private bool bloqueiaPesquisa = false;
        public bool bloqueiaEventosTextChanged = false;
        public FrmManipularPrestacaoDeContas(string statusOperacao)
        {
            this.StatusOperacao = statusOperacao;
            InitializeComponent();
            txtTotalDevolvida.Leave += txtTotalDevolvida_Leave;
            ConfigurarDataGridView();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        private void FrmPrestacaoDeContasDataGrid_Load(object sender, EventArgs e)
        {
            LogUtil.WriteLog($"FrmPrestacaoDeContasDataGrid_Load iniciado com StatusOperacao: {StatusOperacao}");
            if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO" || StatusOperacao == "ESTORNAR")
            {
                CarregarPrestacaoExistente(VendedorID); // Carrega todas as prestações do vendedor
            }
            else
            {
                LogUtil.WriteLog("StatusOperacao inválido para este formulário.");
                MessageBox.Show("Operação inválida para este formulário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            dgvPrestacaoDeContas.CellValueChanged += dgvPrestacaoDeContas_CellValueChanged;
        }
        // Método para carregar uma prestação existente concluída no DatagridView

        //private void CarregarPrestacaoExistente(int vendedorID)
        //{
        //    LogUtil.WriteLog($"Iniciando CarregarPrestacaoExistente com VendedorID: {vendedorID}, StatusOperacao: {StatusOperacao}");
        //    try
        //    {
        //        var prestacaoDAL = new PrestacaoDeContasDAL();
        //        var entregasDAL = new EntregasDal();

        //        LogUtil.WriteLog("Chamando CarregarPrestacoesPorVendedorID...");
        //        var prestacoes = prestacaoDAL.CarregarPrestacoesPorVendedorID(vendedorID);

        //        if (prestacoes.Count == 0)
        //        {
        //            LogUtil.WriteLog("Nenhuma prestação encontrada para este vendedor.");
        //            MessageBox.Show("Nenhuma prestação encontrada para este vendedor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        LogUtil.WriteLog($"Prestações carregadas: {prestacoes.Count}");
        //        dgvPrestacaoDeContas.Rows.Clear();
        //        entregasSelecionadas = new List<EntregasModel>();

        //        foreach (var prestacao in prestacoes)
        //        {
        //            LogUtil.WriteLog($"Chamando CarregarEntregasPorID para EntregaID: {prestacao.EntregaID}");
        //            var entrega = entregasDAL.CarregarEntregasPorID(prestacao.EntregaID);
        //            if (entrega != null)
        //            {
        //                LogUtil.WriteLog($"Entrega carregada: {entrega.EntregaID} - {entrega.Nome}");
        //                entregasSelecionadas.Add(entrega);
        //                double percentualComissao = prestacao.ValorRecebido != 0 ? (prestacao.Comissao / prestacao.ValorRecebido * 100) : 0;

        //                dgvPrestacaoDeContas.Rows.Add(
        //                    entrega.EntregaID,
        //                    entrega.Nome,
        //                    entrega.NomeProduto,
        //                    entrega.QuantidadeEntregue,
        //                    entrega.Preco.ToString("C"),
        //                    entrega.DataEntrega.HasValue ? entrega.DataEntrega.Value.ToString("dd/MM/yyyy") : "",
        //                    prestacao.QuantidadeDevolvida.ToString(),
        //                    prestacao.QuantidadeVendida.ToString(),
        //                    prestacao.ValorRecebido.ToString("C"),
        //                    percentualComissao.ToString("F2"),
        //                    prestacao.Comissao.ToString("C"),
        //                    prestacao.DataPrestacao.ToString("dd/MM/yyyy")
        //                );
        //            }
        //            else
        //            {
        //                LogUtil.WriteLog($"Entrega com ID {prestacao.EntregaID} não encontrada.");
        //                MessageBox.Show($"Entrega com ID {prestacao.EntregaID} não encontrada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            }
        //        }

        //        LogUtil.WriteLog("Adicionando linha de totais...");
        //        AdicionarLinhaTotais();
        //        linhaTotaisAdicionada = true;
        //        AtualizarTotaisNosTextBoxes();
        //        LogUtil.WriteLog("CarregarPrestacaoExistente concluído com sucesso.");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.WriteLog($"Erro ao carregar prestação: {ex.Message}");
        //        MessageBox.Show($"Erro ao carregar prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void CarregarPrestacaoExistente(int vendedorID)
        {
            LogUtil.WriteLog($"Iniciando CarregarPrestacaoExistente com VendedorID: {vendedorID}, StatusOperacao: {StatusOperacao}");
            try
            {
                var prestacaoDAL = new PrestacaoDeContasDAL();
                var entregasDAL = new EntregasDal();

                LogUtil.WriteLog("Chamando CarregarPrestacoesPorVendedorID...");
                var prestacoes = prestacaoDAL.CarregarPrestacoesPorVendedorID(vendedorID);

                if (prestacoes.Count == 0)
                {
                    LogUtil.WriteLog("Nenhuma prestação encontrada para este vendedor.");
                    MessageBox.Show("Nenhuma prestação encontrada para este vendedor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LogUtil.WriteLog($"Prestações carregadas: {prestacoes.Count}");
                dgvPrestacaoDeContas.Rows.Clear();
                entregasSelecionadas = new List<EntregasModel>();

                foreach (var prestacao in prestacoes)
                {
                    LogUtil.WriteLog($"Chamando CarregarEntregasPorID para EntregaID: {prestacao.EntregaID}");
                    var entrega = entregasDAL.CarregarEntregasPorID(prestacao.EntregaID);
                    if (entrega != null)
                    {
                        LogUtil.WriteLog($"Entrega carregada: {entrega.EntregaID} - {entrega.Nome}");
                        entregasSelecionadas.Add(entrega);
                        double percentualComissao = prestacao.ValorRecebido != 0 ? (prestacao.Comissao / prestacao.ValorRecebido * 100) : 0;

                        dgvPrestacaoDeContas.Rows.Add(
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
                    }
                    else
                    {
                        LogUtil.WriteLog($"Entrega com ID {prestacao.EntregaID} não encontrada.");
                        MessageBox.Show($"Entrega com ID {prestacao.EntregaID} não encontrada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Configurar edição apenas no modo ALTERAR
                dgvPrestacaoDeContas.ReadOnly = (StatusOperacao != "ALTERAR");
                dgvPrestacaoDeContas.Columns["PrestacaoID"].Visible = false;

                LogUtil.WriteLog("Adicionando linha de totais...");
                AdicionarLinhaTotais();
                linhaTotaisAdicionada = true;
                AtualizarTotaisNosTextBoxes();
                LogUtil.WriteLog("CarregarPrestacaoExistente concluído com sucesso.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar prestação: {ex.Message}");
                MessageBox.Show($"Erro ao carregar prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //private void ConfigurarDataGridView()
        //{
        //    dgvPrestacaoDeContas.AutoGenerateColumns = false;
        //    dgvPrestacaoDeContas.Columns.Clear();

        //    var colEntregaID = new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "ID\nEntrega", ReadOnly = true, Visible = false };
        //    colEntregaID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colEntregaID);

        //    var colVendedor = new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", ReadOnly = true, Width = 170 };
        //    colVendedor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colVendedor);

        //    var colProduto = new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = "Produto\nNome", ReadOnly = true, Width = 150, Visible = false };
        //    colProduto.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colProduto);

        //    var colQtdEntregue = new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "Bilhetes\nEntregues", ReadOnly = true, Width = 80 };
        //    colQtdEntregue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colQtdEntregue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colQtdEntregue);

        //    var colPrecoUnit = new DataGridViewTextBoxColumn { Name = "PrecoUnit", HeaderText = "Preço\nUnitário", ReadOnly = true, Width = 70 };
        //    colPrecoUnit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    colPrecoUnit.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colPrecoUnit);

        //    var colDataEntrega = new DataGridViewTextBoxColumn { Name = "DataEntrega", HeaderText = "Data\nEntrega", ReadOnly = true, Width = 80 };
        //    colDataEntrega.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colDataEntrega.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colDataEntrega);

        //    var colQtdDevolvida = new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = "Bilhetes\nDevolvidos", ReadOnly = false, Width = 75 };
        //    colQtdDevolvida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colQtdDevolvida.DefaultCellStyle.BackColor = Color.LightBlue;
        //    colQtdDevolvida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colQtdDevolvida);

        //    var colQtdVendida = new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = "Bilhetes\nVendidos", ReadOnly = true, Width = 75 };
        //    colQtdVendida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colQtdVendida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colQtdVendida);

        //    var colValorRecebido = new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = "Valor\nRecebido", ReadOnly = true, Width = 100 };
        //    colValorRecebido.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    colValorRecebido.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colValorRecebido);

        //    var colPercentualComissao = new DataGridViewTextBoxColumn { Name = "PercentualComissao", HeaderText = "%\nComissão", ReadOnly = false, Width = 75 };
        //    colPercentualComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colPercentualComissao.DefaultCellStyle.BackColor = Color.LightBlue;
        //    colPercentualComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colPercentualComissao);

        //    var colComissao = new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", ReadOnly = true, Width = 100 };
        //    colComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    colComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colComissao);

        //    var colDataPrestacao = new DataGridViewTextBoxColumn { Name = "DataPrestacao", HeaderText = "Data\nPrestação", ReadOnly = false, Width = 90 };
        //    colDataPrestacao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    colDataPrestacao.DefaultCellStyle.BackColor = Color.LightBlue;
        //    colDataPrestacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dgvPrestacaoDeContas.Columns.Add(colDataPrestacao);

        //    dgvPrestacaoDeContas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        //}
        private void ConfigurarDataGridView()
        {
            dgvPrestacaoDeContas.AutoGenerateColumns = false;
            dgvPrestacaoDeContas.Columns.Clear();

            // Coluna PrestacaoID (invisível)
            var colPrestacaoID = new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "ID\nPrestação", ReadOnly = true, Visible = false };
            colPrestacaoID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPrestacaoID);

            // Coluna EntregaID
            var colEntregaID = new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "ID\nEntrega", ReadOnly = true, Visible = false };
            colEntregaID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colEntregaID);

            // Coluna Vendedor
            var colVendedor = new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", ReadOnly = true, Width = 170 };
            colVendedor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colVendedor);

            // Coluna Produto
            var colProduto = new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = "Produto\nNome", ReadOnly = true, Width = 150 };
            colProduto.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colProduto);

            // Coluna Quantidade Entregue
            var colQtdEntregue = new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "Bilhetes\nEntregues", ReadOnly = true, Width = 80 };
            colQtdEntregue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdEntregue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdEntregue);

            // Coluna Preço Unitário
            var colPrecoUnit = new DataGridViewTextBoxColumn { Name = "PrecoUnit", HeaderText = "Preço\nUnitário", ReadOnly = true, Width = 70 };
            colPrecoUnit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colPrecoUnit.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPrecoUnit);

            // Coluna Data Entrega
            var colDataEntrega = new DataGridViewTextBoxColumn { Name = "DataEntrega", HeaderText = "Data\nEntrega", ReadOnly = true, Width = 80 };
            colDataEntrega.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDataEntrega.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colDataEntrega);

            // Coluna Quantidade Devolvida
            var colQtdDevolvida = new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = "Bilhetes\nDevolvidos", ReadOnly = false, Width = 75 };
            colQtdDevolvida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdDevolvida.DefaultCellStyle.BackColor = Color.LightBlue;
            colQtdDevolvida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdDevolvida);

            // Coluna Quantidade Vendida
            var colQtdVendida = new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = "Bilhetes\nVendidos", ReadOnly = true, Width = 75 };
            colQtdVendida.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colQtdVendida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colQtdVendida);

            // Coluna Valor Recebido
            var colValorRecebido = new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = "Valor\nRecebido", ReadOnly = true, Width = 100 };
            colValorRecebido.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colValorRecebido.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colValorRecebido);

            // Coluna Percentual Comissão
            var colPercentualComissao = new DataGridViewTextBoxColumn { Name = "PercentualComissao", HeaderText = "%\nComissão", ReadOnly = false, Width = 75 };
            colPercentualComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercentualComissao.DefaultCellStyle.BackColor = Color.LightBlue;
            colPercentualComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colPercentualComissao);

            // Coluna Comissão
            var colComissao = new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", ReadOnly = true, Width = 100 };
            colComissao.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colComissao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrestacaoDeContas.Columns.Add(colComissao);

            // Coluna Data Prestação
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

            foreach (DataGridViewRow row in dgvPrestacaoDeContas.Rows)
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

        private void dgvPrestacaoDeContas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            AtualizarTotaisNosTextBoxes();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAlterarOuExcluir_Click(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                LogUtil.WriteLog("Iniciando btnAlterar_Click...");
                try
                {
                    // Verificar se há uma linha selecionada
                    if (dgvPrestacaoDeContas.SelectedRows.Count == 0)
                    {
                        LogUtil.WriteLog("Nenhuma linha selecionada para alterar.");
                        MessageBox.Show("Selecione uma entrega para alterar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Obter a linha selecionada
                    DataGridViewRow selectedRow = dgvPrestacaoDeContas.SelectedRows[0];
                    int prestacaoID = Convert.ToInt32(selectedRow.Cells["PrestacaoID"].Value);
                    int entregaID = Convert.ToInt32(selectedRow.Cells["EntregaID"].Value);
                    string nomeVendedor = selectedRow.Cells["Nome"].Value.ToString();

                    // Confirmar a alteração com o usuário
                    LogUtil.WriteLog($"Solicitando confirmação para alterar EntregaID: {entregaID}, Vendedor: {nomeVendedor}");
                    DialogResult result = MessageBox.Show(
                        $"Deseja salvar as alterações na prestação de contas da entrega {entregaID} do vendedor {nomeVendedor}?",
                        "Confirmação de Alteração",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        LogUtil.WriteLog("Alteração cancelada pelo usuário.");
                        return;
                    }

                    // Obter os valores editados da linha
                    int quantidadeVendida = Convert.ToInt32(selectedRow.Cells["QuantidadeVendida"].Value);
                    int quantidadeDevolvida = Convert.ToInt32(selectedRow.Cells["QuantidadeDevolvida"].Value);
                    double valorRecebido = Convert.ToDouble(selectedRow.Cells["ValorRecebido"].Value.ToString().Replace("R$", "").Trim());
                    double percentualComissao = Convert.ToDouble(selectedRow.Cells["PercentualComissao"].Value);
                    double comissao = Convert.ToDouble(selectedRow.Cells["Comissao"].Value.ToString().Replace("R$", "").Trim());
                    DateTime dataPrestacao = DateTime.ParseExact(selectedRow.Cells["DataPrestacao"].Value.ToString(), "dd/MM/yyyy", null);

                    // Validar os dados
                    if (quantidadeVendida < 0 || quantidadeDevolvida < 0 || valorRecebido < 0 || comissao < 0)
                    {
                        LogUtil.WriteLog("Valores inválidos detectados na alteração.");
                        MessageBox.Show("Os valores não podem ser negativos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int quantidadeEntregue = Convert.ToInt32(selectedRow.Cells["QuantidadeEntregue"].Value);
                    if (quantidadeVendida + quantidadeDevolvida > quantidadeEntregue)
                    {
                        LogUtil.WriteLog("Soma de Quantidade Vendida e Devolvida excede Quantidade Entregue.");
                        MessageBox.Show("A soma da quantidade vendida e devolvida não pode exceder a quantidade entregue.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Executar a alteração
                    var prestacaoDAL = new PrestacaoDeContasDAL();
                    var fluxoCaixaDAL = new FluxoCaixaDAL();

                    LogUtil.WriteLog($"Alterando prestação para EntregaID: {entregaID}");

                    // 1. Atualizar a prestação de contas
                    prestacaoDAL.AtualizarPrestacaoConcluida(prestacaoID, entregaID, quantidadeVendida, quantidadeDevolvida, valorRecebido, comissao, dataPrestacao);

                    // 2. Atualizar as movimentações do fluxo de caixa
                    fluxoCaixaDAL.AtualizarMovimentacoesPorPrestacao(prestacaoID, valorRecebido, comissao);

                    LogUtil.WriteLog($"Alteração concluída para EntregaID: {entregaID}");
                    MessageBox.Show("Prestação de contas alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteLog($"Erro ao alterar prestação: {ex.Message}");
                    MessageBox.Show($"Erro ao alterar prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (StatusOperacao == "ESTORNAR")
            {
                LogUtil.WriteLog("Iniciando btnEstornar_Click...");
                try
                {
                    if (dgvPrestacaoDeContas.SelectedRows.Count == 0)
                    {
                        LogUtil.WriteLog("Nenhuma linha selecionada para estornar.");
                        MessageBox.Show("Selecione uma entrega para estornar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DataGridViewRow selectedRow = dgvPrestacaoDeContas.SelectedRows[0];
                    int prestacaoID = Convert.ToInt32(selectedRow.Cells["PrestacaoID"].Value);
                    int entregaID = Convert.ToInt32(selectedRow.Cells["EntregaID"].Value);
                    string nomeVendedor = selectedRow.Cells["Nome"].Value.ToString();

                    LogUtil.WriteLog($"Solicitando confirmação para estornar EntregaID: {entregaID}, Vendedor: {nomeVendedor}");
                    DialogResult result = MessageBox.Show(
                        $"Deseja realmente estornar a prestação de contas da entrega {entregaID} do vendedor {nomeVendedor}? " +
                        "Isso reverterá a entrega para pendente e removerá as movimentações financeiras associadas.",
                        "Confirmação de Estorno",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        LogUtil.WriteLog("Estorno cancelado pelo usuário.");
                        return;
                    }

                    var prestacaoDAL = new PrestacaoDeContasDAL();
                    var entregasDAL = new EntregasDal();
                    var fluxoCaixaDAL = new FluxoCaixaDAL();

                    LogUtil.WriteLog($"Estornando prestação para EntregaID: {entregaID}");
                    prestacaoDAL.ExcluirPrestacaoPorEntregaID(entregaID);
                    entregasDAL.MarcarEntregaComoPendente(entregaID);
                    fluxoCaixaDAL.ExcluirMovimentacoesPorPrestacao(prestacaoID);

                    LogUtil.WriteLog($"Estorno concluído para EntregaID: {entregaID}");
                    MessageBox.Show("Prestação de contas estornada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dgvPrestacaoDeContas.Rows.Remove(selectedRow);
                    AtualizarTotaisNosTextBoxes();
                }
                catch (Exception ex)
                {
                    LogUtil.WriteLog($"Erro ao estornar prestação: {ex.Message}");
                    MessageBox.Show($"Erro ao estornar prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (StatusOperacao == "EXCLUSÃO")
            {
                LogUtil.WriteLog("Iniciando btnExcluir_Click...");
                try
                {
                    // Verificar se há uma linha selecionada
                    if (dgvPrestacaoDeContas.SelectedRows.Count == 0)
                    {
                        LogUtil.WriteLog("Nenhuma linha selecionada para excluir.");
                        MessageBox.Show("Selecione uma entrega para excluir.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Obter a linha selecionada
                    DataGridViewRow selectedRow = dgvPrestacaoDeContas.SelectedRows[0];
                    int prestacaoID = Convert.ToInt32(selectedRow.Cells["PrestacaoID"].Value); // Nome da coluna
                    int entregaID = Convert.ToInt32(selectedRow.Cells["EntregaID"].Value);     // Nome da coluna
                    string nomeVendedor = selectedRow.Cells["Nome"].Value.ToString();          // Nome da coluna

                    // Confirmar a exclusão com o usuário
                    LogUtil.WriteLog($"Solicitando confirmação para excluir EntregaID: {entregaID}, Vendedor: {nomeVendedor}");
                    DialogResult result = MessageBox.Show(
                        $"Deseja realmente excluir permanentemente a prestação de contas da entrega {entregaID} do vendedor {nomeVendedor}? " +
                        "Isso removerá a prestação e suas movimentações financeiras do sistema. Esta ação não pode ser desfeita.",
                        "Confirmação de Exclusão",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        LogUtil.WriteLog("Exclusão cancelada pelo usuário.");
                        return;
                    }

                    // Executar a exclusão
                    var prestacaoDAL = new PrestacaoDeContasDAL();
                    var entregasDAL = new EntregasDal();
                    var fluxoCaixaDAL = new FluxoCaixaDAL();

                    LogUtil.WriteLog($"Excluindo prestação para EntregaID: {entregaID}");

                    // 1. Remover a prestação de contas
                    prestacaoDAL.ExcluirPrestacaoPorEntregaID(entregaID);

                    // 2. Remover a entrega associada
                    entregasDAL.ExcluirEntregaPorID(entregaID);

                    // 3. Remover movimentações do fluxo de caixa associadas ao PrestacaoID
                    fluxoCaixaDAL.ExcluirMovimentacoesPorPrestacao(prestacaoID);

                    LogUtil.WriteLog($"Exclusão concluída para EntregaID: {entregaID}");
                    MessageBox.Show("Prestação de contas excluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Atualizar o DataGridView removendo a linha excluída
                    dgvPrestacaoDeContas.Rows.Remove(selectedRow);
                    AtualizarTotaisNosTextBoxes();
                }
                catch (Exception ex)
                {
                    LogUtil.WriteLog($"Erro ao excluir prestação: {ex.Message}");
                    MessageBox.Show($"Erro ao excluir prestação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
