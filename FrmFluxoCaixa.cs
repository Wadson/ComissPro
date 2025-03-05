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
        }
        private void ConfigurarDataGridView()
        {
            dgvFluxoCaixa.AutoGenerateColumns = false;
            dgvFluxoCaixa.Columns.Clear();
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "TipoMovimentacao", HeaderText = "Tipo", Width = 100 });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" } });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataMovimentacao", HeaderText = "Data", Width = 120, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 250 });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "Prestação ID", Width = 80 });

            // Reduzir a altura do cabeçalho
            dgvFluxoCaixa.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; // Impede redimensionamento pelo usuário
            dgvFluxoCaixa.ColumnHeadersHeight = 25; // Define a altura do cabeçalho (padrão é maior, ex.: 36)

            dgvFluxoCaixa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvFluxoCaixa.ReadOnly = true;
            dgvFluxoCaixa.CellFormatting += dgvFluxoCaixa_CellFormatting;
        }
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

        private void btnEntrada_Click(object sender, EventArgs e)
        {
            if (fluxoCaixaDAL.IsCaixaFechado(dtpDataSelecionada.Value))
            {
                MessageBox.Show($"O caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")} está fechado. Não é possível registrar movimentações.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtValorEntrada.Text, out double valor) || valor <= 0)
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
            txtValorEntrada.Text = "";
            txtDescricao.Text = "";
        }

        private void btnSaida_Click(object sender, EventArgs e)
        {
            if (fluxoCaixaDAL.IsCaixaFechado(dtpDataSelecionada.Value))
            {
                MessageBox.Show($"O caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")} está fechado. Não é possível registrar movimentações.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtValorRetirada.Text, out double valor) || valor <= 0)
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
            txtValorRetirada.Text = "";
            txtDescricao.Text = "";
        }

        private void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Deseja fechar o caixa do dia {dtpDataSelecionada.Value.ToString("dd/MM/yyyy")}? Isso arquivará as movimentações.", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fluxoCaixaDAL.FecharCaixaDiario(dtpDataSelecionada.Value); // Passar a data selecionada
                CarregarMovimentacoes();
                MessageBox.Show("Caixa fechado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
