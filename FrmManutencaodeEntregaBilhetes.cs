using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ClosedXML.Excel; // Para Excel
using System.Diagnostics;
using System.IO; // Para abrir os arquivos
using System.Linq;
using static ComissPro.Model;
using static ComissPro.Utilitario;
using System.Data.SQLite;

namespace ComissPro
{
	public partial class FrmManutencaodeEntregaBilhetes : KryptonForm
	{
        private bool bloqueiaEventosTextChanged = false;

        private new string StatusOperacao;
        public FrmManutencaodeEntregaBilhetes(string statusOperacao)
		{
			InitializeComponent();

            // Adiciona o evento DataBindingComplete
            dataGridManutencaoEntregas.DataBindingComplete += dataGridManutencaoEntregas_DataBindingComplete;

            timer1.Interval = 1000; // Confirma o intervalo
            timer1.Tick += timer1_Tick; // Garante que o evento está associado
            txtPesquisa.TextChanged += txtPesquisa_TextChanged; // Adicionar o evento aqui

            this.StatusOperacao = statusOperacao;
            //Centraliza o Label dentro do Panel
            label28.Location = new Point(
                (kryptonPanel2.Width - label28.Width) / 2,
                (kryptonPanel2.Height - label28.Height) / 2);
        }
        public void PersonalizarDataGridView(KryptonDataGridView dgv)
        {
            if (dgv.Columns.Count >= 10) // 10 colunas com Preco
            {
                // Renomeia as colunas
                dgv.Columns[0].Name = "EntregaID";
                dgv.Columns[1].Name = "VendedorID";
                dgv.Columns[2].Name = "Nome";
                dgv.Columns[3].Name = "ProdutoID";
                dgv.Columns[4].Name = "NomeProduto";
                dgv.Columns[5].Name = "QuantidadeEntregue";
                dgv.Columns[6].Name = "DataEntrega";
                dgv.Columns[7].Name = "PrestacaoRealizada";
                dgv.Columns[8].Name = "Preco";
                dgv.Columns[9].Name = "Total";

                // Define larguras fixas específicas para as colunas visíveis
                dgv.Columns["Nome"].Width = 250;
                dgv.Columns["NomeProduto"].Width = 200;
                dgv.Columns["QuantidadeEntregue"].Width = 140;
                dgv.Columns["Preco"].Width = 120;
                dgv.Columns["Total"].Width = 120;
                dgv.Columns["DataEntrega"].Width = 130;

                // Formatar valores monetários (N2) para Preco e Total
                dgv.Columns["Preco"].DefaultCellStyle.Format = "N2";
                dgv.Columns["Total"].DefaultCellStyle.Format = "N2";

                // Formatar DataEntrega como data curta
                dgv.Columns["DataEntrega"].DefaultCellStyle.Format = "d";

                // Centralizar a coluna QuantidadeEntregue
                dgv.Columns["QuantidadeEntregue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Ocultar as colunas não necessárias
                dgv.Columns["VendedorID"].Visible = false;
                dgv.Columns["ProdutoID"].Visible = false;
                dgv.Columns["PrestacaoRealizada"].Visible = false;
                dgv.Columns["EntregaID"].Visible = false;

                // Centralizar cabeçalhos das colunas
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                }
            }

            // Configurações adicionais
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.ReadOnly = true;
            // Removido o DataBindingComplete para gerenciar a linha de soma manualmente
        }
        private void AdicionarLinhaTotais(KryptonDataGridView dgv)
        {
            if (dgv.Rows.Count == 0) return; // Não faz nada se o grid estiver vazio

            // Calcula os totais com tratamento para DBNull
            int totalQuantidade = 0;
            double totalPreco = 0;
            double totalValor = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                totalQuantidade += row.Cells["QuantidadeEntregue"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["QuantidadeEntregue"].Value) : 0;
                totalPreco += row.Cells["Preco"].Value != DBNull.Value ? Convert.ToDouble(row.Cells["Preco"].Value) : 0;
                totalValor += row.Cells["Total"].Value != DBNull.Value ? Convert.ToDouble(row.Cells["Total"].Value) : 0;
            }

            // Adiciona a linha de totais
            int index = dgv.Rows.Add();
            var rowTotal = dgv.Rows[index];
            rowTotal.Cells["Nome"].Value = "Totais";
            rowTotal.Cells["QuantidadeEntregue"].Value = totalQuantidade.ToString();
            rowTotal.Cells["Preco"].Value = totalPreco.ToString("N2");
            rowTotal.Cells["Total"].Value = totalValor.ToString("N2");

            // Estiliza a linha de totais
            rowTotal.DefaultCellStyle.BackColor = Color.LightGray;
            rowTotal.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            rowTotal.ReadOnly = true;
        }
       
        public void CarregarEntregasNoGrid(string nomeVendedor = "")
        {
            try
            {
                EntregasDal objetoDAL = new EntregasDal();
                DataTable dt;

                // Se houver texto de pesquisa, usa o método de pesquisa; caso contrário, lista todas as entregas
                if (!string.IsNullOrWhiteSpace(nomeVendedor))
                {
                    dt = objetoDAL.PesquisaVendasAbertasPorVendedor(nomeVendedor);
                }
                else
                {
                    dt = objetoDAL.listaEntregas();
                }

                dataGridManutencaoEntregas.DataSource = dt;

                // Ajustar cabeçalhos
                dataGridManutencaoEntregas.Columns["EntregaID"].HeaderText = "ID Entrega";
                dataGridManutencaoEntregas.Columns["Nome"].HeaderText = "Vendedor";
                dataGridManutencaoEntregas.Columns["NomeProduto"].HeaderText = "Produto";
                dataGridManutencaoEntregas.Columns["QuantidadeEntregue"].HeaderText = "Quantidade (Un)";
                dataGridManutencaoEntregas.Columns["Preco"].HeaderText = "Preço Unitário";
                dataGridManutencaoEntregas.Columns["Total"].HeaderText = "Total";
                dataGridManutencaoEntregas.Columns["DataEntrega"].HeaderText = "Data";

                // Ocultar colunas desnecessárias
                dataGridManutencaoEntregas.Columns["VendedorID"].Visible = false;
                dataGridManutencaoEntregas.Columns["ProdutoID"].Visible = false;
                dataGridManutencaoEntregas.Columns["PrestacaoRealizada"].Visible = false;

                PersonalizarDataGridView(dataGridManutencaoEntregas);

                // Rolar até a última linha para mostrar os totais
                if (dataGridManutencaoEntregas.Rows.Count > 0)
                {
                    int ultimaLinha = dataGridManutencaoEntregas.Rows.Count - 1;
                    dataGridManutencaoEntregas.FirstDisplayedScrollingRowIndex = ultimaLinha;
                }

                // Atualizar o total de registros (exclui a linha de totais)
                int totalRegistros = dataGridManutencaoEntregas.Rows.Count - 1;
                lblTotalRegistros.Text = $"Total de Registros: {totalRegistros}";
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro ao carregar entregas: {ex.Message}");
                MessageBox.Show("Erro ao carregar entregas: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTotalRegistros.Text = "Total de Registros: 0";
            }
        }




        public void PesquisarEntregasPorVendedor(string nomeVendedor)
        {
            LogUtil.WriteLog($"Iniciando PesquisarEntregasPorVendedor com NomeVendedor: {nomeVendedor}");
            try
            {
                var entregasDAL = new EntregasDal();
                DataTable dt = entregasDAL.PesquisaVendasAbertasPorVendedor(nomeVendedor);

                // Limpar o DataGridView
                dataGridManutencaoEntregas.Rows.Clear();

                // Preencher o DataGridView com os resultados
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Nome"].ToString() == "Totais")
                    {
                        // Linha de totais
                        dataGridManutencaoEntregas.Rows.Add(
                            row["EntregaID"],
                            row["VendedorID"],
                            row["Nome"],
                            row["ProdutoID"],
                            row["NomeProduto"],
                            row["QuantidadeEntregue"],
                            row["DataEntrega"],
                            row["PrestacaoRealizada"],
                            row["Preco"],
                            row["Total"].ToString() == "" ? "0.00" : Convert.ToDouble(row["Total"]).ToString("N2")
                        );
                        // Estilizar a linha de totais
                        int lastRowIndex = dataGridManutencaoEntregas.Rows.Count - 1;
                        dataGridManutencaoEntregas.Rows[lastRowIndex].DefaultCellStyle.BackColor = Color.DarkGray;
                        dataGridManutencaoEntregas.Rows[lastRowIndex].DefaultCellStyle.ForeColor = Color.White;
                        dataGridManutencaoEntregas.Rows[lastRowIndex].DefaultCellStyle.Font = new Font(dataGridManutencaoEntregas.Font, FontStyle.Bold);
                    }
                    else
                    {
                        // Linhas de dados (entregas pendentes)
                        dataGridManutencaoEntregas.Rows.Add(
                            row["EntregaID"],
                            row["VendedorID"],
                            row["Nome"],
                            row["ProdutoID"],
                            row["NomeProduto"],
                            row["QuantidadeEntregue"],
                            row["DataEntrega"] != DBNull.Value ? Convert.ToDateTime(row["DataEntrega"]).ToString("dd/MM/yyyy") : "",
                            row["PrestacaoRealizada"],
                            Convert.ToDouble(row["Preco"]).ToString("N2"),
                            Convert.ToDouble(row["Total"]).ToString("N2")
                        );
                    }
                }

                // Personalizar o DataGridView após adicionar as linhas
                PersonalizarDataGridView(dataGridManutencaoEntregas);

                // Atualizar o total de registros (exclui a linha de totais)
                int totalRegistros = dataGridManutencaoEntregas.Rows.Count - 1;
                lblTotalRegistros.Text = $"Total de Registros: {totalRegistros}";

                LogUtil.WriteLog($"PesquisarEntregasPorVendedor concluído com {dt.Rows.Count} linhas.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em PesquisarEntregasPorVendedor: {ex.Message}");
                MessageBox.Show($"Erro ao pesquisar entregas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar para carregar prestações por EntregaID



        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.Trim();
            LogUtil.WriteLog($"txtPesquisa_TextChanged disparado com texto: {textoPesquisa}");
            Listar(textoPesquisa); // Aqui não passamos forcarTodos explicitamente
        }
        public void Listar(string nomeVendedor = "", bool forcarTodos = false)
        {
            if (forcarTodos)
            {
                CarregarEntregasNoGrid(""); // Força a exibição de todos os registros, ignorando o filtro
            }
            else
            {
                CarregarEntregasNoGrid(nomeVendedor); // Usa o filtro do txtPesquisa
            }
        }
        private void CarregaDados()
        {
            FrmCadastroDeEntregas formEntregas = new FrmCadastroDeEntregas(StatusOperacao);

            if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO")
            {
                try
                {
                    if (dataGridManutencaoEntregas.Rows.Count == 0)
                    {
                        MessageBox.Show("A DataGridView está vazia. Não há dados para serem processados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    formEntregas.bloqueiaEventosTextChanged = true;

                    // Preencher os campos com dados do grid
                    formEntregas.txtNomeVendedor.Text = dataGridManutencaoEntregas.CurrentRow.Cells["Nome"].Value?.ToString() ?? "Vendedor Excluído";
                    formEntregas.txtNomeProduto.Text = dataGridManutencaoEntregas.CurrentRow.Cells["NomeProduto"].Value?.ToString() ?? "Produto Desconhecido";
                    formEntregas.txtQuantidade.Text = dataGridManutencaoEntregas.CurrentRow.Cells["QuantidadeEntregue"].Value?.ToString() ?? "0"; // Quantidade em unidades
                    formEntregas.txtPrecoUnit.Text = dataGridManutencaoEntregas.CurrentRow.Cells["Preco"].Value?.ToString() ?? "0.00"; // Preço por unidade
                    formEntregas.txtTotal.Text = dataGridManutencaoEntregas.CurrentRow.Cells["Total"].Value?.ToString() ?? "0.00";
                    formEntregas.dtpDataEntregaBilhete.Text = dataGridManutencaoEntregas.CurrentRow.Cells["DataEntrega"].Value?.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy");
                    formEntregas.txtEntregaID.Text = dataGridManutencaoEntregas.CurrentRow.Cells["EntregaID"].Value?.ToString() ?? "0";

                    formEntregas.VendedorID = int.TryParse(dataGridManutencaoEntregas.CurrentRow.Cells["VendedorID"].Value?.ToString(), out int vendedorID) ? vendedorID : -1;
                    formEntregas.ProdutoID = int.TryParse(dataGridManutencaoEntregas.CurrentRow.Cells["ProdutoID"].Value?.ToString(), out int produtoID) ? produtoID : -1;

                    if (StatusOperacao == "ALTERAR")
                    {
                        formEntregas.lblStatus.Text = "Alterar: quantidade e valor";
                        formEntregas.btnSalvar.Text = "Alterar";
                        formEntregas.btnSalvar.ForeColor = Color.Orange;
                        formEntregas.btnSalvar.BackColor = Color.OrangeRed;
                        formEntregas.btnNovo.Enabled = false;
                        formEntregas.txtNomeVendedor.Enabled = false;
                        formEntregas.txtNomeProduto.Enabled = false;
                        formEntregas.btnLocalizarVendedor.Enabled = false;
                        formEntregas.btnLocalizarProduto.Enabled = false;                        
                    }
                    else if (StatusOperacao == "EXCLUSÃO")
                    {
                        formEntregas.lblStatus.Text = "Excluir registro!";
                        formEntregas.lblStatus.ForeColor = Color.Red;
                        formEntregas.btnSalvar.Text = "Excluir";

                        formEntregas.txtEntregaID.Enabled = false;
                        formEntregas.txtNomeVendedor.Enabled = false;
                        formEntregas.txtNomeProduto.Enabled = false;
                        formEntregas.txtQuantidade.Enabled = false;
                        formEntregas.txtPrecoUnit.Enabled = false;
                        formEntregas.txtTotal.Enabled = false;
                    }

                    formEntregas.bloqueiaEventosTextChanged = false;
                    formEntregas.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (StatusOperacao == "NOVO")
            {
                formEntregas.lblStatus.Text = "ENTREGA DE BILHETES";
                formEntregas.ShowDialog();
            }
        }
        // Exportar para Excel
        private void ExportarParaExcel()
        {
            try
            {
                DataTable dt = (DataTable)dataGridManutencaoEntregas.DataSource;
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    sfd.FileName = "Entregas_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Entregas");
                            worksheet.Cell(1, 1).InsertTable(dt);
                            worksheet.Columns().AdjustToContents();
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
                MessageBox.Show("Erro ao exportar para Excel: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }               
       
        private void FrmManutençãodeEntregaBilhetes_Load(object sender, EventArgs e)
        {
            LogUtil.WriteLog("FrmManutencaodeEntregaBilhetes_Load iniciado.");
            Listar(); // Carrega todas as entregas sem filtro
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridManutencaoEntregas);
        }


        public void HabilitarTimer(bool habilitar)
        {
            if (habilitar)
            {
                Listar("", true); // Força a atualização completa
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Listar();
            timer1.Enabled = false;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarParaExcel();
        }

        private void btnRelatorios_Click(object sender, EventArgs e)
        {           
        }

        private void btnAltera_Click(object sender, EventArgs e)
        {
            StatusOperacao = "ALTERAR";
            CarregaDados();
        }

        private void btnExclui_Click(object sender, EventArgs e)
        {
            StatusOperacao = "EXCLUSÃO";
            CarregaDados();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            StatusOperacao = "NOVO";
            CarregaDados();
        }

        private void dataGridManutencaoEntregas_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridManutencaoEntregas.Rows.Count > 0)
            {
                int ultimaLinha = dataGridManutencaoEntregas.Rows.Count - 1;
                dataGridManutencaoEntregas.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridManutencaoEntregas.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dataGridManutencaoEntregas.Font, FontStyle.Bold);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LogUtil.WriteLog("FrmManutencaodeEntregaBilhetes_Load iniciado.");
            Listar(); // Carrega todas as entregas sem filtro
            Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridManutencaoEntregas);
        }

        private void btnPrestacaoDeContas_Click(object sender, EventArgs e)
        {
            FrmPrestacaoDeContasDataGrid formPrestacao = new FrmPrestacaoDeContasDataGrid(StatusOperacao);
            formPrestacao.ShowDialog();
        }
    }
}
