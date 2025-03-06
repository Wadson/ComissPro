using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;
using static ComissPro.Utilitario;
using System.Data.SQLite;
using ClosedXML.Excel;
using System.Diagnostics;

namespace ComissPro
{
    public partial class FrmFluxoCaixa : KryptonForm
    {
        private readonly FluxoCaixaDAL fluxoCaixaDAL = new FluxoCaixaDAL();
        public FrmFluxoCaixa()
        {
            InitializeComponent();

            ConfigurarDataGridView();
            dtpDataSelecionada.Value = DateTime.Today; // Data inicial é hoje
            CarregarMovimentacoes();
            AtualizarSaldo();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        private void ConfigurarDataGridView()
        {
            dgvFluxoCaixa.AutoGenerateColumns = false;
            dgvFluxoCaixa.Columns.Clear();

            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "TipoMovimentacao", HeaderText = "Tipo", Width = 100 });

            // Coluna Valor - Bloquear ordenação
            var colunaValor = new DataGridViewTextBoxColumn
            {
                Name = "Valor",
                HeaderText = "Valor",
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" },
                SortMode = DataGridViewColumnSortMode.NotSortable // Desativa ordenação
            };
            dgvFluxoCaixa.Columns.Add(colunaValor);

            // Coluna DataMovimentacao - Bloquear ordenação
            var colunaData = new DataGridViewTextBoxColumn
            {
                Name = "DataMovimentacao",
                HeaderText = "Data",
                Width = 120,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" },
                SortMode = DataGridViewColumnSortMode.NotSortable // Desativa ordenação
            };
            dgvFluxoCaixa.Columns.Add(colunaData);

            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 350 });

            // Coluna PrestacaoID - Ocultar
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrestacaoID",
                HeaderText = "Prestação ID",
                Width = 80,
                Visible = false // Oculta a coluna
            });

            dgvFluxoCaixa.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvFluxoCaixa.ColumnHeadersHeight = 25;

            dgvFluxoCaixa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvFluxoCaixa.ReadOnly = true;
            dgvFluxoCaixa.CellFormatting += dgvFluxoCaixa_CellFormatting;
        }
        //private void ConfigurarDataGridView()
        //{
        //    dgvFluxoCaixa.AutoGenerateColumns = false;
        //    dgvFluxoCaixa.Columns.Clear();
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "TipoMovimentacao", HeaderText = "Tipo", Width = 100 });
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" } });
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataMovimentacao", HeaderText = "Data", Width = 120, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 250 });
        //    dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "Prestação ID", Width = 80 });

        //    // Reduzir a altura do cabeçalho
        //    dgvFluxoCaixa.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; // Impede redimensionamento pelo usuário
        //    dgvFluxoCaixa.ColumnHeadersHeight = 25; // Define a altura do cabeçalho (padrão é maior, ex.: 36)

        //    dgvFluxoCaixa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        //    dgvFluxoCaixa.ReadOnly = true;
        //    dgvFluxoCaixa.CellFormatting += dgvFluxoCaixa_CellFormatting;
        //}
        private void CarregarMovimentacoes()
        {
            LogUtil.WriteLog("Carregando movimentações no FrmFluxoCaixa...");
            try
            {
                var movimentacoes = fluxoCaixaDAL.ObterMovimentacoesPorData(dtpDataSelecionada.Value);

                dgvFluxoCaixa.Rows.Clear();

                double totalEntradas = 0;
                double totalSaidas = 0;

                foreach (var m in movimentacoes)
                {
                    dgvFluxoCaixa.Rows.Add(
                        m.FluxoCaixaID,
                        m.TipoMovimentacao,
                        m.Valor,
                        m.DataMovimentacao,
                        m.Descricao,
                        m.PrestacaoID.HasValue ? m.PrestacaoID.ToString() : ""
                    );

                    if (m.TipoMovimentacao == "ENTRADA")
                        totalEntradas += m.Valor;
                    else if (m.TipoMovimentacao == "SAÍDA")
                        totalSaidas += m.Valor;
                }

                int totalIndex = dgvFluxoCaixa.Rows.Add();
                var totalRow = dgvFluxoCaixa.Rows[totalIndex];
                totalRow.Cells["TipoMovimentacao"].Value = "Totais";
                totalRow.Cells["Valor"].Value = (totalEntradas - totalSaidas).ToString("C");
                totalRow.Cells["DataMovimentacao"].Value = "";
                totalRow.Cells["Descricao"].Value = "";
                totalRow.Cells["PrestacaoID"].Value = "";

                txtTotalEntradas.Text = totalEntradas > 0 ? totalEntradas.ToString("N2") : "0,00";
                txtTotalSaidas.Text = totalSaidas > 0 ? totalSaidas.ToString("N2") : "0,00";
                AtualizarSaldo();
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar movimentações: {ex.Message}");
                MessageBox.Show($"Erro ao carregar movimentações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarSaldo()
        {
            double saldo = fluxoCaixaDAL.CalcularSaldoPorData(dtpDataSelecionada.Value);
            txtSaldo.Text = saldo.ToString("C");
        }





        public double CalcularSaldoDoDia()
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = @"
            SELECT 
                (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'ENTRADA' AND DATE(DataMovimentacao) = @DataAtual) -
                (SELECT COALESCE(SUM(Valor), 0) FROM FluxoCaixa WHERE TipoMovimentacao = 'SAÍDA' AND DATE(DataMovimentacao) = @DataAtual) AS Saldo";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataAtual", DateTime.Today.ToString("yyyy-MM-dd"));
                    var result = cmd.ExecuteScalar();
                    double saldo = result == DBNull.Value ? 0 : Convert.ToDouble(result);
                    LogUtil.WriteLog($"Saldo calculado do dia: {saldo}");
                    return saldo;
                }
            }
        }


        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvFluxoCaixa_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dgvFluxoCaixa.Rows[e.RowIndex];
                string tipo = row.Cells["TipoMovimentacao"].Value?.ToString();

                if (tipo == "ENTRADA")
                    e.CellStyle.ForeColor = Color.Blue;
                else if (tipo == "SAÍDA")
                    e.CellStyle.ForeColor = Color.Red;
                else if (tipo == "Totais")
                {
                    e.CellStyle.BackColor = Color.LightGray;
                    e.CellStyle.Font = new Font(dgvFluxoCaixa.Font, FontStyle.Bold);
                }
            }
        }

        private void FrmFluxoCaixa_KeyDown(object sender, KeyEventArgs e)
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

        private void dtpDataSelecionada_ValueChanged(object sender, EventArgs e)
        {
            CarregarMovimentacoes();
        }

        private void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            // Verifica se há movimentações reais no DataGrid
            bool temMovimentacoes = false;
            foreach (DataGridViewRow row in dgvFluxoCaixa.Rows)
            {
                // Ignora a linha "Totais" (ajuste o critério conforme sua implementação)
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() != "Totais")
                {
                    temMovimentacoes = true;
                    break; // Encontrou uma movimentação, não precisa continuar
                }
            }

            if (!temMovimentacoes)
            {
                MessageBox.Show("O caixa está vazio. Não há movimentações para fechar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Sai do método se não houver movimentações
            }
            if (MessageBox.Show($"Deseja fechar o caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")}? Isso arquivará as movimentações.", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fluxoCaixaDAL.FecharCaixaDiario(dtpDataSelecionada.Value); // Passar a data selecionada
                CarregarMovimentacoes();
                MessageBox.Show("Caixa fechado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (cmbTipoMovimento.Text == "Entrada")
            {
                if (fluxoCaixaDAL.IsCaixaFechado(dtpDataSelecionada.Value))
                {
                    MessageBox.Show($"O caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")} está fechado. Não é possível registrar movimentações.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtValorEntradaSaida.Text, out double valor) || valor <= 0)
                {
                    MessageBox.Show("Digite um valor válido para a entrada!");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDescricao.Text))
                {
                    MessageBox.Show("Informe a descrição da entrada!");
                    return;
                }

                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "ENTRADA",
                    Valor = valor,
                    Descricao = txtDescricao.Text,
                    DataMovimentacao = DateTime.Now
                });

                CarregarMovimentacoes();
                txtValorEntradaSaida.Text = "";
                txtDescricao.Text = "";
                gbMovimentoCaixa.Visible = false;
                cmbTipoMovimento.Text = "";
            }
            if (cmbTipoMovimento.Text == "Saída")
            {
                if (fluxoCaixaDAL.IsCaixaFechado(dtpDataSelecionada.Value))
                {
                    MessageBox.Show($"O caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")} está fechado. Não é possível registrar movimentações.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtValorEntradaSaida.Text, out double valor) || valor <= 0)
                {
                    MessageBox.Show("Digite um valor válido para a retirada!");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDescricao.Text))
                {
                    MessageBox.Show("Informe a descrição da retirada!");
                    return;
                }

                fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
                {
                    TipoMovimentacao = "SAÍDA",
                    Valor = valor,
                    Descricao = txtDescricao.Text,
                    DataMovimentacao = DateTime.Now
                });

                CarregarMovimentacoes();
                txtValorEntradaSaida.Text = "";
                txtDescricao.Text = "";
                gbMovimentoCaixa.Visible = false;
                cmbTipoMovimento.Text = "";
            }
            else
            { 
            }
        }

        private void cmbTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipoMovimento.Text == "Entrada")
            {
                gbMovimentoCaixa.Visible = true; // Torna o GroupBox visível
                lblTipo.Text = "Valor da Entrada:";
                lblTipo.ForeColor = Color.FromArgb(0, 76, 172);
                txtValorEntradaSaida.Focus(); // Dá foco ao txtValorEntradaSaida
            }
            else if (cmbTipoMovimento.Text == "Saída")
            {
                gbMovimentoCaixa.Visible = true; // Torna o GroupBox visível
                lblTipo.Text = "Valor da Saída:";
                lblTipo.ForeColor = Color.Red;
                txtValorEntradaSaida.Focus(); // Dá foco ao txtValorEntradaSaida
            }
            else
            {
                gbMovimentoCaixa.Visible = false; // Esconde o GroupBox para outros valores
            }
        }

        private void FrmFluxoCaixa_Load(object sender, EventArgs e)
        {
            gbMovimentoCaixa.Visible = false;
        }
        private void ExportarParaExcel()
        {
            try
            {
                // Verificar se o grid tem linhas
                if (dgvFluxoCaixa.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Criar um DataTable para os dados do DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("TipoMovimentacao", typeof(string));
                dt.Columns.Add("Valor", typeof(double));
                dt.Columns.Add("DataMovimentacao", typeof(string));
                dt.Columns.Add("Descricao", typeof(string));
                dt.Columns.Add("PrestacaoID", typeof(string));
                dt.Columns.Add("FluxoCaixaID", typeof(int)); // Será ocultada

                // Preencher o DataTable com os dados do DataGridView
                foreach (DataGridViewRow row in dgvFluxoCaixa.Rows)
                {
                    // Ignorar a linha "Totais" do DataGridView
                    if (row.Cells["TipoMovimentacao"].Value?.ToString() == "Totais")
                        continue;

                    DataRow dr = dt.NewRow();
                    dr["TipoMovimentacao"] = row.Cells["TipoMovimentacao"].Value?.ToString() ?? "";
                    dr["Valor"] = row.Cells["Valor"].Value != null ? Convert.ToDouble(row.Cells["Valor"].Value.ToString().Replace("R$", "").Trim()) : 0.0;
                    dr["DataMovimentacao"] = row.Cells["DataMovimentacao"].Value?.ToString() ?? "";
                    dr["Descricao"] = row.Cells["Descricao"].Value?.ToString() ?? "";
                    dr["PrestacaoID"] = row.Cells["PrestacaoID"].Value?.ToString() ?? "";
                    dr["FluxoCaixaID"] = row.Cells["FluxoCaixaID"].Value != null ? Convert.ToInt32(row.Cells["FluxoCaixaID"].Value) : 0;

                    dt.Rows.Add(dr);
                }

                // Exportar para Excel
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    sfd.FileName = "FluxoCaixa_" + dtpDataSelecionada.Value.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("FluxoCaixa");

                            // Inserir o título
                            worksheet.Cell(1, 1).Value = $"Fluxo de Caixa - {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")}";
                            worksheet.Cell(1, 1).Style.Font.Bold = true;
                            worksheet.Range(1, 1, 1, 5).Merge(); // Mesclar células para o título

                            // Inserir a tabela de movimentações
                            worksheet.Cell(3, 1).InsertTable(dt);
                            worksheet.Column(6).Hide(); // Ocultar a coluna FluxoCaixaID (coluna 6)

                            // Adicionar Total Entradas, Total Saídas e Saldo
                            int ultimaLinhaTabela = dt.Rows.Count + 4; // 3 (título e cabeçalho) + 1 (espaço)
                            worksheet.Cell(ultimaLinhaTabela, 1).Value = "Total Entradas";
                            worksheet.Cell(ultimaLinhaTabela, 2).Value = Convert.ToDouble(txtTotalEntradas.Text.Replace("R$", "").Trim());
                            worksheet.Cell(ultimaLinhaTabela, 2).Style.NumberFormat.Format = "R$ #,##0.00";

                            worksheet.Cell(ultimaLinhaTabela + 1, 1).Value = "Total Saídas";
                            worksheet.Cell(ultimaLinhaTabela + 1, 2).Value = Convert.ToDouble(txtTotalSaidas.Text.Replace("R$", "").Trim());
                            worksheet.Cell(ultimaLinhaTabela + 1, 2).Style.NumberFormat.Format = "R$ #,##0.00";

                            worksheet.Cell(ultimaLinhaTabela + 2, 1).Value = "Saldo";
                            worksheet.Cell(ultimaLinhaTabela + 2, 2).Value = Convert.ToDouble(txtSaldo.Text.Replace("R$", "").Trim());
                            worksheet.Cell(ultimaLinhaTabela + 2, 2).Style.NumberFormat.Format = "R$ #,##0.00";

                            // Estilizar os totais
                            worksheet.Range(ultimaLinhaTabela, 1, ultimaLinhaTabela + 2, 1).Style.Font.Bold = true;
                            worksheet.Range(ultimaLinhaTabela, 2, ultimaLinhaTabela + 2, 2).Style.Font.Bold = true;

                            // Ajustar largura das colunas
                            worksheet.Columns().AdjustToContents();

                            // Salvar o arquivo
                            workbook.SaveAs(sfd.FileName);
                        }
                        MessageBox.Show("Exportado para Excel com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Abrir o arquivo gerado
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao exportar para Excel: {ex.Message}");
                MessageBox.Show("Erro ao exportar para Excel: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarParaExcel();
        }
    }
}
