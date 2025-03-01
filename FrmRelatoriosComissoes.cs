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
        private enum TipoRelatorio
        {
            ComissoesPagas,
            EntregasPendentes,
            DesempenhoVendas,
            GeralVendasComissoes
        }
        public FrmRelatoriosComissoes()
        {
            InitializeComponent();            
            ConfigurarColunasComissoesPagas();
        }
        private void ConfigurarColunasComissoesPagas()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VendedorID",
                HeaderText = "ID Vend.",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeVendedor", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DataPrestacao", HeaderText = "Data Prest.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeVendida",
                HeaderText = "Qtd. Vend.",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeDevolvida",
                HeaderText = "Qtd. Dev.",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrestacaoID", HeaderText = "ID Prestação", Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Visible = false });
        }

        private void ConfigurarColunasEntregasPendentes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VendedorID",
                HeaderText = "ID Vend.",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeVendedor", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Width = 80 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeEntregue",
                HeaderText = "Qtd. Entregue",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DataEntrega", HeaderText = "Data Entrega", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeProduto", HeaderText = "Produto", Width = 250 });
        }

        private void ConfigurarColunasDesempenhoVendas()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VendedorID",
                HeaderText = "ID Vend.",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeVendedor", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Width = 80 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeEntregue",
                HeaderText = "Qtd. Entregue",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeVendida",
                HeaderText = "Qtd. Vend.",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QuantidadeDevolvida",
                HeaderText = "Qtd. Dev.",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        }

        private void ConfigurarColunasGeralVendasComissoes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VendedorID",
                HeaderText = "ID Vend.",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeVendedor", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalEntregue",
                HeaderText = "Total Entregue",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalVendido",
                HeaderText = "Total Vendido",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalDevolvido",
                HeaderText = "Total Devolvido",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalRecebido", HeaderText = "Total Recebido", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalComissao", HeaderText = "Total Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        }

        private void AdicionarLinhaTotais(List<object> relatorioComTotais, TipoRelatorio tipo)
        {
            switch (tipo)
            {
                case TipoRelatorio.ComissoesPagas:
                    var comissoes = relatorioComTotais.Cast<PrestacaoContasModel>().ToList();
                    relatorioComTotais.Add(new PrestacaoContasModel
                    {
                        NomeVendedor = "Totais",
                        Comissao = comissoes.Sum(p => p.Comissao),
                        QuantidadeVendida = comissoes.Sum(p => p.QuantidadeVendida),
                        QuantidadeDevolvida = comissoes.Sum(p => p.QuantidadeDevolvida),
                        ValorRecebido = comissoes.Sum(p => p.ValorRecebido)
                    });
                    break;
                case TipoRelatorio.EntregasPendentes:
                    var pendentes = relatorioComTotais.Cast<EntregasModel>().ToList();
                    relatorioComTotais.Add(new EntregasModel
                    {
                        NomeVendedor = "Totais",
                        QuantidadeEntregue = pendentes.Sum(p => p.QuantidadeEntregue)
                    });
                    break;
                case TipoRelatorio.DesempenhoVendas:
                    var desempenho = relatorioComTotais.Cast<DesempenhoVendasModel>().ToList();
                    relatorioComTotais.Add(new DesempenhoVendasModel
                    {
                        NomeVendedor = "Totais",
                        QuantidadeEntregue = desempenho.Sum(p => p.QuantidadeEntregue),
                        QuantidadeVendida = desempenho.Sum(p => p.QuantidadeVendida),
                        QuantidadeDevolvida = desempenho.Sum(p => p.QuantidadeDevolvida),
                        ValorRecebido = desempenho.Sum(p => p.ValorRecebido),
                        Comissao = desempenho.Sum(p => p.Comissao)
                    });
                    break;
                case TipoRelatorio.GeralVendasComissoes:
                    var geral = relatorioComTotais.Cast<GeralVendasComissoesModel>().ToList();
                    relatorioComTotais.Add(new GeralVendasComissoesModel
                    {
                        NomeVendedor = "Totais",
                        TotalEntregue = geral.Sum(p => p.TotalEntregue),
                        TotalVendido = geral.Sum(p => p.TotalVendido),
                        TotalDevolvido = geral.Sum(p => p.TotalDevolvido),
                        TotalRecebido = geral.Sum(p => p.TotalRecebido),
                        TotalComissao = geral.Sum(p => p.TotalComissao)
                    });
                    break;
            }

            // Estilizar a linha de totais após associar o DataSource
            // Estilizar a linha de totais após associar o DataSource
            dgvRelatorio.DataBindingComplete += (s, ev) =>
            {
                int ultimaLinha = dgvRelatorio.Rows.Count - 1;
                if (ultimaLinha >= 0)
                {
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dgvRelatorio.Font, FontStyle.Bold);
                }
            };
        }

        private void btnLocalizar_Click(object sender, EventArgs e, ComboBox cmbTipoRelatorio, DateTimePicker dtpDataInicio, DateTimePicker dtpDataFim, TextBox txtVendedor)
        {
            try
            {
                var dal = new EntregasDal();
                TipoRelatorio tipo = (TipoRelatorio)cmbTipoRelatorio.SelectedIndex;

                object relatorio = null;
                switch (tipo)
                {
                    case TipoRelatorio.ComissoesPagas:
                        ConfigurarColunasComissoesPagas();
                        relatorio = dal.RelatorioComissoesPagas(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                    case TipoRelatorio.EntregasPendentes:
                        ConfigurarColunasEntregasPendentes();
                        relatorio = dal.RelatorioEntregasPendentes(txtVendedor.Text);
                        break;
                    case TipoRelatorio.DesempenhoVendas:
                        ConfigurarColunasDesempenhoVendas();
                        relatorio = dal.RelatorioDesempenhoVendas(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                    case TipoRelatorio.GeralVendasComissoes:
                        ConfigurarColunasGeralVendasComissoes();
                        relatorio = dal.RelatorioGeralVendasComissoes(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                }

                if (relatorio == null || ((IList<object>)relatorio).Count == 0)
                {
                    MessageBox.Show("Nenhum dado encontrado para os filtros especificados.");
                    dgvRelatorio.DataSource = null;
                }
                else
                {
                    var lista = ((IList<object>)relatorio).Cast<object>().ToList();
                    var relatorioComTotais = new List<object>(lista);
                    AdicionarLinhaTotais(relatorioComTotais, tipo);
                    dgvRelatorio.DataSource = relatorioComTotais;
                    MessageBox.Show($"Encontrados {lista.Count - 1} registros.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o relatório: {ex.Message}");
            }
        }

        private void FrmRelatoriosComissoes_Load(object sender, EventArgs e)
        {
            dtpDataInicio.Value = DateTime.Now.AddMonths(-12);
            dtpDataFim.Value = DateTime.Now;
            cmbTipoRelatorio.Items.Clear();
            cmbTipoRelatorio.Items.AddRange(new object[] { "Comissões Pagas", "Entregas Pendentes", "Desempenho de Vendas", "Geral de Vendas e Comissões" });
            cmbTipoRelatorio.SelectedIndex = 0;
            ConfigurarColunasComissoesPagas();
        }

        private void btnLocalizar_Click(object sender, EventArgs e)
        {
            try
            {
                var dal = new EntregasDal();
                TipoRelatorio tipo = (TipoRelatorio)cmbTipoRelatorio.SelectedIndex;

                object relatorio = null;
                switch (tipo)
                {
                    case TipoRelatorio.ComissoesPagas:
                        ConfigurarColunasComissoesPagas();
                        relatorio = dal.RelatorioComissoesPagas(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                    case TipoRelatorio.EntregasPendentes:
                        ConfigurarColunasEntregasPendentes();
                        relatorio = dal.RelatorioEntregasPendentes(txtVendedor.Text);
                        break;
                    case TipoRelatorio.DesempenhoVendas:
                        ConfigurarColunasDesempenhoVendas();
                        relatorio = dal.RelatorioDesempenhoVendas(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                    case TipoRelatorio.GeralVendasComissoes:
                        ConfigurarColunasGeralVendasComissoes();
                        relatorio = dal.RelatorioGeralVendasComissoes(dtpDataInicio.Value, dtpDataFim.Value, txtVendedor.Text);
                        break;
                }

                if (relatorio == null || ((System.Collections.IList)relatorio).Count == 0)
                {
                    MessageBox.Show("Nenhum dado encontrado para os filtros especificados.");
                    dgvRelatorio.DataSource = null;
                }
                else
                {
                    var lista = ((System.Collections.IList)relatorio).Cast<object>().ToList();
                    var relatorioComTotais = new List<object>(lista);
                    AdicionarLinhaTotais(relatorioComTotais, tipo);
                    dgvRelatorio.DataSource = relatorioComTotais;
                    //MessageBox.Show($"Encontrados {lista.Count - 1} registros.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o relatório: {ex.Message}");
            }
        }
    }
}
