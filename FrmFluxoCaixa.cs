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

namespace ComissPro
{
    public partial class FrmFluxoCaixa : KryptonForm
    {
        private readonly FluxoCaixaDAL fluxoCaixaDAL = new FluxoCaixaDAL();
        public FrmFluxoCaixa()
        {
            InitializeComponent();

            ConfigurarDataGridViews();
            AtualizarSaldo();
            CarregarMovimentacoes();
        }

        private void ConfigurarDataGridViews()
        {
            // Configurar dgvEntradas
            dgvEntradas.AutoGenerateColumns = false;
            dgvEntradas.Columns.Clear();
            dgvEntradas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
            dgvEntradas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" } });
            dgvEntradas.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataMovimentacao", HeaderText = "Data", Width = 120, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
            dgvEntradas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 200 });
            dgvEntradas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "Prestação ID", Width = 80 });

            // Configurar dgvSaidas
            dgvSaidas.AutoGenerateColumns = false;
            dgvSaidas.Columns.Clear();
            dgvSaidas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FluxoCaixaID", HeaderText = "ID", Visible = false });
            dgvSaidas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C" } });
            dgvSaidas.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataMovimentacao", HeaderText = "Data", Width = 120, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
            dgvSaidas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", Width = 200 });
            dgvSaidas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "Prestação ID", Width = 80 });
        }

        private void CarregarMovimentacoes()
        {
            var movimentacoes = fluxoCaixaDAL.ObterMovimentacoesDoDia();

            // Limpar ambos os DataGridViews
            dgvEntradas.Rows.Clear();
            dgvSaidas.Rows.Clear();

            // Variáveis para somar os totais
            double totalEntradas = 0;
            double totalRetiradas = 0;

            // Separar entradas e saídas e calcular totais
            foreach (var m in movimentacoes)
            {
                if (m.TipoMovimentacao == "ENTRADA")
                {
                    dgvEntradas.Rows.Add(
                        m.FluxoCaixaID,
                        m.Valor,
                        m.DataMovimentacao,
                        m.Descricao,
                        m.PrestacaoID.HasValue ? m.PrestacaoID.ToString() : ""
                    );
                    totalEntradas += m.Valor;
                }
                else if (m.TipoMovimentacao == "SAIDA")
                {
                    dgvSaidas.Rows.Add(
                        m.FluxoCaixaID,
                        m.Valor,
                        m.DataMovimentacao,
                        m.Descricao,
                        m.PrestacaoID.HasValue ? m.PrestacaoID.ToString() : ""
                    );
                    totalRetiradas += m.Valor;
                }
            }

            // Atualizar os TextBoxes com formato N2, ou "0,00" se vazio
            txtTotalEntrada.Text = totalEntradas > 0 ? totalEntradas.ToString("N2") : "0,00";
            txtTotalRetiradas.Text = totalRetiradas > 0 ? totalRetiradas.ToString("N2") : "0,00";
        }

        private void AtualizarSaldo()
        {
            double saldo = fluxoCaixaDAL.CalcularSaldoDoDia();
            txtSaldoAtual.Text = saldo.ToString("C");
        }

        private void btnRegistrarRetirada_Click(object sender, EventArgs e)
        {

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
                TipoMovimentacao = "SAIDA",
                Valor = valor,
                Descricao = txtDescricao.Text,
                DataMovimentacao = DateTime.Now // Garante que a data seja do dia atual
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
                DataMovimentacao = DateTime.Now // Garante que a data seja do dia atual
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
    }
}
