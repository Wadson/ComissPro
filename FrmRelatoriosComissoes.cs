using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;
using static ComissPro.VendedorDAL;

namespace ComissPro
{
    public partial class FrmRelatoriosComissoes : KryptonForm
    {
        public FrmRelatoriosComissoes()
        {
            InitializeComponent();
            ConfigurarComponentes();
        }
        private void ConfigurarComponentes()
        {
            // Configurar DataGridView
            dgvRelatorio.AutoGenerateColumns = false;            
            dgvRelatorio.AllowUserToAddRows = false; // Evita linha extra em branco
            dgvRelatorio.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRelatorio.ColumnHeadersHeight = 30; // Ajustar altura dos cabeçalhos

            // Definir colunas na ordem desejada com largura reduzida
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VendedorID",
                HeaderText = "ID Vend.",
                Name = "VendedorID",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                Width = 100 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NomeVendedor",
                HeaderText = "Nome Vendedor",
                Name = "NomeVendedor",
                Width = 300 // Fixo
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Comissao",
                HeaderText = "Comissão",
                Name = "Comissao",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" },
                Width = 100 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataPrestacao",
                HeaderText = "Data Prest.",
                Name = "DataPrestacao",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                Width = 100 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeVendida",
                HeaderText = "Qtd. Vend.",
                Name = "QuantidadeVendida",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                Width = 160 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeDevolvida",
                HeaderText = "Qtd. Dev.",
                Name = "QuantidadeDevolvida",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                Width = 160 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorRecebido",
                HeaderText = "Valor Rec.",
                Name = "ValorRecebido",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" },
                Width = 100 // Reduzido ainda mais
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrestacaoID",
                HeaderText = "ID Prestação",
                Name = "PrestacaoID",
                Visible = false
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EntregaID",
                HeaderText = "ID Entrega",
                Name = "EntregaID",
                Visible = false
            });

            // Configurar DateTimePickers para valores padrão amplos
            dtpDataInicio.Value = DateTime.Now.AddMonths(-12); // Último ano
            dtpDataFim.Value = DateTime.Now; // Hoje
        }
        private void AdicionarLinhaTotais(List<PrestacaoContasModel> relatorio)
        {
            // Calcular totais
            double totalComissao = relatorio.Sum(p => p.Comissao);
            int totalQtdVendida = relatorio.Sum(p => p.QuantidadeVendida);
            int totalQtdDevolvida = relatorio.Sum(p => p.QuantidadeDevolvida);
            double totalValorRecebido = relatorio.Sum(p => p.ValorRecebido);

            // Adicionar linha ao DataGridView
            dgvRelatorio.Rows.Add(
                "", // VendedorID (vazio para totais)
                "Totais", // NomeVendedor
                totalComissao.ToString("C2"), // Comissao
                "", // DataPrestacao (vazio)
                totalQtdVendida, // QuantidadeVendida
                totalQtdDevolvida, // QuantidadeDevolvida
                totalValorRecebido.ToString("C2"), // ValorRecebido
                "", // PrestacaoID (oculto)
                ""  // EntregaID (oculto)
            );

            // Estilizar a linha de totais
            int ultimaLinha = dgvRelatorio.Rows.Count - 1;
            dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.LightGray;
            dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dgvRelatorio.Font, FontStyle.Bold);
        }
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            try
            {
                var dal = new EntregasDAL();
                var relatorio = dal.RelatorioComissoesPagas(
                    dtpDataInicio.Value,
                    dtpDataFim.Value,
                    string.IsNullOrWhiteSpace(txtVendedor.Text) ? null : txtVendedor.Text
                );

                if (relatorio == null || relatorio.Count == 0)
                {
                    MessageBox.Show("Nenhum dado encontrado para os filtros especificados.");
                    dgvRelatorio.DataSource = null;
                }
                else
                {
                    // Criar uma lista combinada com dados e totais
                    var relatorioComTotais = new List<PrestacaoContasModel>(relatorio);
                    var linhaTotal = new PrestacaoContasModel
                    {
                        NomeVendedor = "Totais",
                        Comissao = relatorio.Sum(p => p.Comissao),
                        QuantidadeVendida = relatorio.Sum(p => p.QuantidadeVendida),
                        QuantidadeDevolvida = relatorio.Sum(p => p.QuantidadeDevolvida),
                        ValorRecebido = relatorio.Sum(p => p.ValorRecebido)
                    };
                    relatorioComTotais.Add(linhaTotal);

                    dgvRelatorio.DataSource = relatorioComTotais;

                    // Estilizar a linha de totais
                    int ultimaLinha = dgvRelatorio.Rows.Count - 1;
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dgvRelatorio.Font, FontStyle.Bold);

                    //MessageBox.Show($"Encontrados {relatorio.Count} registros.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o relatório: {ex.Message}");
            }
        }   
    }
}
