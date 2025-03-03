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
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DataPrestacao", HeaderText = "Data Prest.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeVendida", HeaderText = "Qtd. Vend.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeDevolvida", HeaderText = "Qtd. Dev.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrestacaoID", HeaderText = "ID Prestação", Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Visible = false });
        }

        private void ConfigurarColunasEntregasPendentes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Width = 80, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProdutoID", HeaderText = "ID Produto", Width = 80, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeEntregue", HeaderText = "Qtd. Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeVendida", HeaderText = "Qtd. Vendida", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeDevolvida", HeaderText = "Qtd. Devolvida", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DataEntrega", HeaderText = "Data Entrega", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeProduto", HeaderText = "Produto", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Preco", HeaderText = "Preço Unitário", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Preço Total", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Recebido", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        }

        private void ConfigurarColunasDesempenhoVendas()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Width = 80 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeEntregue", HeaderText = "Qtd. Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeVendida", HeaderText = "Qtd. Vend.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeDevolvida", HeaderText = "Qtd. Dev.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        }

        private void ConfigurarColunasGeralVendasComissoes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalEntregue", HeaderText = "Total Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalVendido", HeaderText = "Total Vendido", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalDevolvido", HeaderText = "Total Devolvido", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
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
                        Nome = "Totais",
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
                        Nome = "Totais",
                        QuantidadeEntregue = pendentes.Sum(p => p.QuantidadeEntregue),
                        QuantidadeVendida = pendentes.Sum(p => p.QuantidadeVendida),
                        QuantidadeDevolvida = pendentes.Sum(p => p.QuantidadeDevolvida),
                        Total = pendentes.Sum(p => p.Total),
                        ValorRecebido = pendentes.Sum(p => p.ValorRecebido)
                    });
                    break;
                case TipoRelatorio.DesempenhoVendas:
                    var desempenho = relatorioComTotais.Cast<DesempenhoVendasModel>().ToList();
                    relatorioComTotais.Add(new DesempenhoVendasModel
                    {
                        Nome = "Totais",
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
                        Nome = "Totais",
                        TotalEntregue = geral.Sum(p => p.TotalEntregue),
                        TotalVendido = geral.Sum(p => p.TotalVendido),
                        TotalDevolvido = geral.Sum(p => p.TotalDevolvido),
                        TotalRecebido = geral.Sum(p => p.TotalRecebido),
                        TotalComissao = geral.Sum(p => p.TotalComissao)
                    });
                    break;
            }

            // Garantir que o evento seja registrado apenas uma vez
            dgvRelatorio.DataBindingComplete -= DgvRelatorio_DataBindingComplete;
            dgvRelatorio.DataBindingComplete += DgvRelatorio_DataBindingComplete;
        }


        private void DgvRelatorio_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Verificar se há linhas e se a última linha é "Totais"
            if (dgvRelatorio.Rows.Count > 0)
            {
                int ultimaLinha = dgvRelatorio.Rows.Count - 1;
                var nomeCell = dgvRelatorio.Rows[ultimaLinha].Cells["Nome"];
                if (nomeCell != null && nomeCell.Value?.ToString() == "Totais")
                {
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvRelatorio.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dgvRelatorio.Font, FontStyle.Bold);
                }
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


        //JÁ ESTAVA AQUI
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
                    MessageBox.Show("Nenhum dado encontrado para os filtros especificados.","Informação!",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                    dgvRelatorio.DataSource = null;
                }
                else
                {
                    var lista = ((System.Collections.IList)relatorio).Cast<object>().ToList();
                    var relatorioComTotais = new List<object>(lista);
                    AdicionarLinhaTotais(relatorioComTotais, tipo);
                    dgvRelatorio.DataSource = null; // Limpar antes de associar
                    dgvRelatorio.DataSource = relatorioComTotais;
                    //MessageBox.Show($"Encontrados {lista.Count} registros (excluindo totais).", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk););
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o relatório: {ex.Message}", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvRelatorio_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }
    }
}
