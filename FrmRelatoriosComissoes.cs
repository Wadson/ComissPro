using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;
using static ComissPro.Utilitario;
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
        private readonly EntregasDal entregasDal = new EntregasDal();
        private readonly VendedorDAL vendedorDal = new VendedorDAL();

        public FrmRelatoriosComissoes()
        {
            InitializeComponent();
            ConfigurarColunasComissoesPagas();
            CarregarVendedores();
        }



        private void ConfigurarColunasComissoesPagas()
        {
            LogUtil.WriteLog($"Colunas antes de limpar: {string.Join(", ", dgvRelatorio.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText))}");
            dgvRelatorio.AutoGenerateColumns = false; // Adicionar esta linha para evitar colunas automáticas
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DataPrestacao", HeaderText = "Data Prest.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeVendida", HeaderText = "Qtd. Vend.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeDevolvida", HeaderText = "Qtd. Dev.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeEntregue", HeaderText = "Qtd. Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NomeProduto", HeaderText = "Produto", Width = 200 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Preco", HeaderText = "Preço Unit.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrestacaoID", HeaderText = "ID Prestação", Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Visible = false });

            ConfigurarEstiloCabeçalho();
        }

        private void ConfigurarColunasEntregasPendentes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, Visible = false });
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

            ConfigurarEstiloCabeçalho();
        }

        private void ConfigurarColunasDesempenhoVendas()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntregaID", HeaderText = "ID Entrega", Width = 80, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeEntregue", HeaderText = "Qtd. Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeVendida", HeaderText = "Qtd. Vend.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QuantidadeDevolvida", HeaderText = "Qtd. Dev.", Width = 60, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ValorRecebido", HeaderText = "Valor Rec.", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Comissao", HeaderText = "Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            ConfigurarEstiloCabeçalho();
        }

        private void ConfigurarColunasGeralVendasComissoes()
        {
            dgvRelatorio.Columns.Clear();
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VendedorID", HeaderText = "ID Vend.", Width = 50, Visible = false });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome Vendedor", Width = 250 });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalEntregue", HeaderText = "Total Entregue", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalVendido", HeaderText = "Total Vendido", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalDevolvido", HeaderText = "Total Devolvido", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalRecebido", HeaderText = "Total Recebido", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalComissao", HeaderText = "Total Comissão", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            ConfigurarEstiloCabeçalho();
        }

        private void ConfigurarEstiloCabeçalho()
        {
            foreach (DataGridViewColumn column in dgvRelatorio.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
            }
            dgvRelatorio.ColumnHeadersHeight = 25;
            dgvRelatorio.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void ConfigurarTotais()
        {
            txtTotalEntregue.TextAlign = HorizontalAlignment.Center;
            txtTotalQuantidadeVendida.TextAlign = HorizontalAlignment.Center;
            txtTotalQuantidadeDevolvida.TextAlign = HorizontalAlignment.Center;
            txtTotalValorRecebido.TextAlign = HorizontalAlignment.Center;
            txtTotalComissao.TextAlign = HorizontalAlignment.Center;
        }

        private void CarregarVendedores()
        {
            try
            {
                var vendedores = vendedorDal.listaVendedor(); // Assume que retorna DataTable
                cmbVendedores.Items.Clear();
                cmbVendedores.Items.Add("Todos os Vendedores"); // Opção padrão

                foreach (DataRow vendedor in vendedores.Rows)
                {
                    cmbVendedores.Items.Add(new { VendedorID = Convert.ToInt32(vendedor["VendedorID"]), Nome = vendedor["Nome"].ToString() });
                }

                cmbVendedores.DisplayMember = "Nome";
                cmbVendedores.ValueMember = "VendedorID";
                cmbVendedores.SelectedIndex = 0; // Seleciona "Todos os Vendedores"
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar vendedores: {ex.Message}");
                MessageBox.Show($"Erro ao carregar vendedores: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }








        private void FrmRelatoriosComissoes_Load(object sender, EventArgs e)
        {
            dtpDataInicio.Value = DateTime.Today.AddMonths(-12); // Já está assim, mas confirmamos
            dtpDataFim.Value = DateTime.Today; // Já está assim
            LogUtil.WriteLog($"dtpDataInicio inicial: {dtpDataInicio.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
            LogUtil.WriteLog($"dtpDataFim inicial: {dtpDataFim.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
            cmbTipoRelatorio.Items.Clear();
            cmbTipoRelatorio.Items.AddRange(new object[] { "Comissões Pagas", "Entregas Pendentes", "Desempenho de Vendas", "Geral de Vendas e Comissões" });
            cmbTipoRelatorio.SelectedIndex = 0;
            ConfigurarColunasComissoesPagas();
            CarregarVendedores();
            ConfigurarTotais();
        }
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            try
            {
                TipoRelatorio tipo = (TipoRelatorio)cmbTipoRelatorio.SelectedIndex;
                string nomeVendedor = null;
                if (cmbVendedores.SelectedIndex > 0) // Se não for "Todos os Vendedores"
                {
                    dynamic vendedorSelecionado = cmbVendedores.SelectedItem;
                    nomeVendedor = vendedorSelecionado.Nome;
                }

                switch (tipo)
                {
                    case TipoRelatorio.ComissoesPagas:
                        ConfigurarColunasComissoesPagas();
                        LogUtil.WriteLog($"Filtro aplicado - DataInicio: {dtpDataInicio.Value.ToString("yyyy-MM-dd HH:mm:ss")}, DataFim: {dtpDataFim.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                        var comissoes = entregasDal.RelatorioComissoesPagas(dtpDataInicio.Value, dtpDataFim.Value, nomeVendedor);
                        if (comissoes == null || comissoes.Count == 0)
                        {
                            MessageBox.Show("Nenhum dado encontrado para os filtros especificados.", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            dgvRelatorio.DataSource = null;
                            LimparTotais();
                        }
                        else
                        {
                            LogUtil.WriteLog($"Relatório Comissões Pagas: {comissoes.Count} registros encontrados.");
                            foreach (var item in comissoes)
                            {
                                LogUtil.WriteLog($"PrestacaoID={item.PrestacaoID}, QuantidadeEntregue={item.QuantidadeEntregue}, Total={item.Total}");
                            }
                            dgvRelatorio.DataSource = null;
                            dgvRelatorio.Rows.Clear();
                            dgvRelatorio.DataSource = comissoes;
                            AtualizarTotaisComissoesPagas(comissoes);
                        }
                        break;

                    case TipoRelatorio.EntregasPendentes:
                        ConfigurarColunasEntregasPendentes();
                        var pendentes = entregasDal.RelatorioEntregasPendentes(nomeVendedor);
                        if (pendentes == null || pendentes.Count == 0)
                        {
                            MessageBox.Show("Nenhum dado encontrado para os filtros especificados.", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            dgvRelatorio.DataSource = null;
                            LimparTotais();
                        }
                        else
                        {
                            dgvRelatorio.DataSource = null;
                            dgvRelatorio.DataSource = pendentes;
                            AtualizarTotaisEntregasPendentes(pendentes);
                        }
                        break;

                    case TipoRelatorio.DesempenhoVendas:
                        ConfigurarColunasDesempenhoVendas();
                        var desempenho = entregasDal.RelatorioDesempenhoVendas(dtpDataInicio.Value, dtpDataFim.Value, nomeVendedor);
                        if (desempenho == null || desempenho.Count == 0)
                        {
                            MessageBox.Show("Nenhum dado encontrado para os filtros especificados.", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            dgvRelatorio.DataSource = null;
                            LimparTotais();
                        }
                        else
                        {
                            dgvRelatorio.DataSource = null;
                            dgvRelatorio.DataSource = desempenho;
                            AtualizarTotaisDesempenhoVendas(desempenho);
                        }
                        break;

                    case TipoRelatorio.GeralVendasComissoes:
                        ConfigurarColunasGeralVendasComissoes();
                        var geral = entregasDal.RelatorioGeralVendasComissoes(dtpDataInicio.Value, dtpDataFim.Value, nomeVendedor);
                        if (geral == null || geral.Count == 0)
                        {
                            MessageBox.Show("Nenhum dado encontrado para os filtros especificados.", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            dgvRelatorio.DataSource = null;
                            LimparTotais();
                        }
                        else
                        {
                            dgvRelatorio.DataSource = null;
                            dgvRelatorio.DataSource = geral;
                            AtualizarTotaisGeralVendasComissoes(geral);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o relatório: {ex.Message}", "Informação!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }



        private void AtualizarTotaisComissoesPagas(List<PrestacaoContasModel> comissoes)
        {
            txtTotalEntregue.Text = comissoes.Sum(p => p.QuantidadeEntregue).ToString("N0");
            txtTotalQuantidadeVendida.Text = comissoes.Sum(p => p.QuantidadeVendida).ToString("N0");
            txtTotalQuantidadeDevolvida.Text = comissoes.Sum(p => p.QuantidadeDevolvida).ToString("N0");
            txtTotalValorRecebido.Text = comissoes.Sum(p => p.ValorRecebido).ToString("N2"); // Alterado para N2
            txtTotalComissao.Text = comissoes.Sum(p => p.Comissao).ToString("N2"); // Alterado para N2
        }

        private void AtualizarTotaisEntregasPendentes(List<EntregasModel> pendentes)
        {
            txtTotalEntregue.Text = pendentes.Sum(p => p.QuantidadeEntregue).ToString("N0");
            txtTotalQuantidadeVendida.Text = pendentes.Sum(p => p.QuantidadeVendida).ToString("N0");
            txtTotalQuantidadeDevolvida.Text = pendentes.Sum(p => p.QuantidadeDevolvida).ToString("N0");
            txtTotalValorRecebido.Text = pendentes.Sum(p => p.ValorRecebido).ToString("C2");
            txtTotalComissao.Text = "N/A"; // Não aplicável neste relatório
        }

        private void AtualizarTotaisDesempenhoVendas(List<DesempenhoVendasModel> desempenho)
        {
            txtTotalEntregue.Text = desempenho.Sum(p => p.QuantidadeEntregue).ToString("N0");
            txtTotalQuantidadeVendida.Text = desempenho.Sum(p => p.QuantidadeVendida).ToString("N0");
            txtTotalQuantidadeDevolvida.Text = desempenho.Sum(p => p.QuantidadeDevolvida).ToString("N0");
            txtTotalValorRecebido.Text = desempenho.Sum(p => p.ValorRecebido).ToString("C2");
            txtTotalComissao.Text = desempenho.Sum(p => p.Comissao).ToString("C2");
        }

        private void AtualizarTotaisGeralVendasComissoes(List<GeralVendasComissoesModel> geral)
        {
            txtTotalEntregue.Text = geral.Sum(p => p.TotalEntregue).ToString("N0");
            txtTotalQuantidadeVendida.Text = geral.Sum(p => p.TotalVendido).ToString("N0");
            txtTotalQuantidadeDevolvida.Text = geral.Sum(p => p.TotalDevolvido).ToString("N0");
            txtTotalValorRecebido.Text = geral.Sum(p => p.TotalRecebido).ToString("C2");
            txtTotalComissao.Text = geral.Sum(p => p.TotalComissao).ToString("C2");
        }

        private void LimparTotais()
        {
            txtTotalEntregue.Text = "0";
            txtTotalQuantidadeVendida.Text = "0";
            txtTotalQuantidadeDevolvida.Text = "0";
            txtTotalValorRecebido.Text = "0,00";
            txtTotalComissao.Text = "0,00";
        }






        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvRelatorio.Rows.Count == 0)
            {
                MessageBox.Show("Nenhum dado para exportar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", FileName = $"Relatorio_{cmbTipoRelatorio.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Relatorio");
                        int colIndex = 1;
                        for (int i = 0; i < dgvRelatorio.Columns.Count; i++)
                            if (dgvRelatorio.Columns[i].Visible)
                            {
                                worksheet.Cell(1, colIndex).Value = dgvRelatorio.Columns[i].HeaderText;
                                colIndex++;
                            }
                        for (int i = 0; i < dgvRelatorio.Rows.Count; i++)
                        {
                            colIndex = 1;
                            for (int j = 0; j < dgvRelatorio.Columns.Count; j++)
                                if (dgvRelatorio.Columns[j].Visible)
                                {
                                    worksheet.Cell(i + 2, colIndex).Value = dgvRelatorio.Rows[i].Cells[j].Value?.ToString();
                                    colIndex++;
                                }
                        }
                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }
                    Process.Start(new ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
                    MessageBox.Show("Relatório exportado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
