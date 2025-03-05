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
            CarregarMovimentacoes();
            AtualizarSaldo();
        }
        private void ConfigurarDataGridView()
        {
            // Configurar o único DataGridView
            dgvFluxoCaixa.AutoGenerateColumns = false;
            dgvFluxoCaixa.Columns.Clear();
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "TipoMovimentacao", HeaderText = "Tipo", Width = 100 });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" } });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataMovimentacao", HeaderText = "Data", Width = 120, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 250 });
            dgvFluxoCaixa.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "Prestação ID", Width = 80 });

            // Configurações adicionais
            dgvFluxoCaixa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvFluxoCaixa.ReadOnly = true;

            // Evento para colorir as linhas
            dgvFluxoCaixa.CellFormatting += dgvFluxoCaixa_CellFormatting;
        }
        private void CarregarMovimentacoes()
        {
            LogUtil.WriteLog("Carregando movimentações do dia no FrmFluxoCaixa...");
            try
            {
                var movimentacoes = fluxoCaixaDAL.ObterMovimentacoesDoDia();

                // Limpar o DataGridView
                dgvFluxoCaixa.Rows.Clear();

                // Variáveis para somar os totais
                double totalEntradas = 0;
                double totalSaidas = 0;

                // Preencher o DataGridView com as movimentações
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

                // Adicionar a linha de totais
                int totalIndex = dgvFluxoCaixa.Rows.Add();
                var totalRow = dgvFluxoCaixa.Rows[totalIndex];
                totalRow.Cells["TipoMovimentacao"].Value = "Totais";
                totalRow.Cells["Valor"].Value = (totalEntradas - totalSaidas).ToString("N2");
                totalRow.Cells["DataMovimentacao"].Value = "";
                totalRow.Cells["Descricao"].Value = "";
                totalRow.Cells["PrestacaoID"].Value = "";

                // Atualizar os TextBoxes
                txtTotalEntradas.Text = totalEntradas > 0 ? totalEntradas.ToString("N2") : "0,00";
                txtTotalSaidas.Text = totalSaidas > 0 ? totalSaidas.ToString("N2") : "0,00";
                AtualizarSaldo(); // Já está chamando o método corrigido
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar movimentações: {ex.Message}");
                MessageBox.Show($"Erro ao carregar movimentações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarSaldo()
        {
            double saldo = fluxoCaixaDAL.CalcularSaldoDoDia();
            txtSaldo.Text = saldo.ToString("N2");
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


        private void btnRegistrarRetirada_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtValorRetirada.Text, out double valor) || valor <= 0)
            {
                MessageBox.Show("Digite um valor válido para a retirada!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe a descrição da retirada!","Atenção!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
            {
                TipoMovimentacao = "SAÍDA",
                Valor = valor,
                Descricao = txtDescricao.Text,
                DataMovimentacao = DateTime.Now
            });

            AtualizarSaldo();
            CarregarMovimentacoes();
            txtValorRetirada.Text = "";
            txtDescricao.Text = "";
        }

        private void btnRegistrarEntrada_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtValorEntrada.Text, out double valor) || valor <= 0)
            {
                MessageBox.Show("Digite um valor válido para a entrada!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe a descrição da entrada!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            fluxoCaixaDAL.RegistrarMovimentacao(new FluxoCaixaModel
            {
                TipoMovimentacao = "ENTRADA",
                Valor = valor,
                Descricao = txtDescricao.Text,
                DataMovimentacao = DateTime.Now
            });

            AtualizarSaldo();
            CarregarMovimentacoes();
            txtValorEntrada.Text = "";
            txtDescricao.Text = "";
        }

        private void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja fechar o caixa do dia? Isso limpará as movimentações de hoje.", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fluxoCaixaDAL.FecharCaixaDiario();
                AtualizarSaldo();
                CarregarMovimentacoes();
                MessageBox.Show("Caixa fechado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                {
                    e.CellStyle.ForeColor = Color.Blue;
                }
                else if (tipo == "SAÍDA")
                {
                    e.CellStyle.ForeColor = Color.Red;
                }
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
    }
}
