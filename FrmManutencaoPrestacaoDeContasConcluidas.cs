using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Utilitario;

namespace ComissPro
{
    public partial class FrmManutencaoPrestacaoDeContasConcluidas : KryptonForm
    {
        private string StatusOperacao;
        public FrmManutencaoPrestacaoDeContasConcluidas(string statusOperacao)
        {
            InitializeComponent();
            this.StatusOperacao = statusOperacao;
            txtPesquisa.TextChanged += txtPesquisa_TextChanged; // Vincular o evento
            dataGridPrestacaoContas.DataBindingComplete += DataGridPrestacaoContas_DataBindingComplete;
        }
        private void Log(string message)
        {
            File.AppendAllText("Log em FrmManutencaoPrestacaoDeContasConcluidas.txt", $"{DateTime.Now}: {message}\n");
        }
      
        public void Listar(string nomeVendedor = "")
        {
            Log("Listar() iniciado. em FrmManutencaoPrestacaoDeContasConcluidas");
            PesquisarPrestaçõesPorVendedor(nomeVendedor);
            Log("Listar() finalizado.");
        }
        private void ConfigurarColunasDataGridView()
        {
            LogUtil.WriteLog("Configurando colunas do dataGridPrestacaoContas.");
            dataGridPrestacaoContas.AutoGenerateColumns = false;
            dataGridPrestacaoContas.Columns.Clear();

            // Adicionar colunas manualmente
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Vendedor", Width = 200 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeEntregue", HeaderText = "Qtd. Entregue", Width = 140 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "NomeProduto", HeaderText = "Produto", Width = 200 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Preco", HeaderText = "Preço Unitário", Width = 120 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeVendida", HeaderText = "Qtd. Vendida", Width = 140 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantidadeDevolvida", HeaderText = "Qtd. Devolvida", Width = 140 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "ValorRecebido", HeaderText = "Valor Recebido", Width = 120 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comissao", HeaderText = "Comissão", Width = 120 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataPrestacao", HeaderText = "Data Prestação", Width = 130 });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "EntregaID", HeaderText = "EntregaID", Visible = false });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrestacaoID", HeaderText = "PrestacaoID", Visible = false });
            dataGridPrestacaoContas.Columns.Add(new DataGridViewTextBoxColumn { Name = "VendedorID", HeaderText = "VendedorID", Visible = false });

            // Aplicar personalizações adicionais
            PersonalizarDataGridView();
        }


        public void PersonalizarDataGridView()
        {
            if (dataGridPrestacaoContas.Columns.Count >= 9) // Temos 9 colunas visíveis
            {
                // Renomeia as colunas na ordem solicitada (já feito no ConfigurarColunasDataGridView, mas mantido por segurança)
                dataGridPrestacaoContas.Columns[0].Name = "Nome";
                dataGridPrestacaoContas.Columns[1].Name = "QuantidadeEntregue";
                dataGridPrestacaoContas.Columns[2].Name = "NomeProduto";
                dataGridPrestacaoContas.Columns[3].Name = "Preco";
                dataGridPrestacaoContas.Columns[4].Name = "QuantidadeVendida";
                dataGridPrestacaoContas.Columns[5].Name = "QuantidadeDevolvida";
                dataGridPrestacaoContas.Columns[6].Name = "ValorRecebido";
                dataGridPrestacaoContas.Columns[7].Name = "Comissao";
                dataGridPrestacaoContas.Columns[8].Name = "DataPrestacao";

                // Ocultar colunas extras
                dataGridPrestacaoContas.Columns["EntregaID"].Visible = false;
                dataGridPrestacaoContas.Columns["PrestacaoID"].Visible = false;
                dataGridPrestacaoContas.Columns["VendedorID"].Visible = false;

                // Define larguras fixas específicas para as colunas
                dataGridPrestacaoContas.Columns["Nome"].Width = 200;
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].Width = 140;
                dataGridPrestacaoContas.Columns["NomeProduto"].Width = 200;
                dataGridPrestacaoContas.Columns["Preco"].Width = 120;
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].Width = 140;
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].Width = 140;
                dataGridPrestacaoContas.Columns["ValorRecebido"].Width = 120;
                dataGridPrestacaoContas.Columns["Comissao"].Width = 120;
                dataGridPrestacaoContas.Columns["DataPrestacao"].Width = 130;

                // Define cabeçalhos visíveis
                dataGridPrestacaoContas.Columns["Nome"].HeaderText = "Vendedor";
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].HeaderText = "Qtd. Entregue";
                dataGridPrestacaoContas.Columns["NomeProduto"].HeaderText = "Produto";
                dataGridPrestacaoContas.Columns["Preco"].HeaderText = "Preço Unitário";
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].HeaderText = "Qtd. Vendida";
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].HeaderText = "Qtd. Devolvida";
                dataGridPrestacaoContas.Columns["ValorRecebido"].HeaderText = "Valor Recebido";
                dataGridPrestacaoContas.Columns["Comissao"].HeaderText = "Comissão";
                dataGridPrestacaoContas.Columns["DataPrestacao"].HeaderText = "Data Prestação";

                // Formatar valores monetários (N2)
                dataGridPrestacaoContas.Columns["Preco"].DefaultCellStyle.Format = "N2";
                dataGridPrestacaoContas.Columns["ValorRecebido"].DefaultCellStyle.Format = "N2";
                dataGridPrestacaoContas.Columns["Comissao"].DefaultCellStyle.Format = "N2";

                // Formatar DataPrestacao como data curta e tratar DBNull
                dataGridPrestacaoContas.Columns["DataPrestacao"].DefaultCellStyle.Format = "d";
                dataGridPrestacaoContas.Columns["DataPrestacao"].DefaultCellStyle.NullValue = "";

                // Centralizar colunas numéricas
                dataGridPrestacaoContas.Columns["QuantidadeEntregue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["QuantidadeVendida"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["QuantidadeDevolvida"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["Preco"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["ValorRecebido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridPrestacaoContas.Columns["Comissao"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Centralizar cabeçalhos das colunas
                foreach (DataGridViewColumn column in dataGridPrestacaoContas.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                }
            }

            // Configurações adicionais
            dataGridPrestacaoContas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridPrestacaoContas.ReadOnly = true;
        }
        private void PesquisarPrestaçõesPorVendedor(string nomeVendedor)
        {
            LogUtil.WriteLog($"Iniciando PesquisarPrestaçõesPorVendedor com NomeVendedor: {nomeVendedor}");
            try
            {
                var prestacaoDAL = new PrestacaoDeContasDAL();
                DataTable dt = prestacaoDAL.PesquisaVendasConcluidasPorVendedor(nomeVendedor);

                // Limpar o DataGridView
                dataGridPrestacaoContas.Rows.Clear();

                // Preencher o DataGridView com os resultados
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Nome"].ToString() == "TOTAIS")
                    {
                        // Linha de totais
                        int index = dataGridPrestacaoContas.Rows.Add(
                            row["Nome"],
                            row["QuantidadeEntregue"],
                            row["NomeProduto"],
                            row["Preco"],
                            row["QuantidadeVendida"],
                            row["QuantidadeDevolvida"],
                            row["ValorRecebido"].ToString() == "" ? "0.00" : Convert.ToDouble(row["ValorRecebido"]).ToString("N2"),
                            row["Comissao"].ToString() == "" ? "0.00" : Convert.ToDouble(row["Comissao"]).ToString("N2"),
                            row["DataPrestacao"],
                            row["EntregaID"],
                            row["PrestacaoID"],
                            row["VendedorID"]
                        );

                        // Estilizar a linha de totais diretamente
                        var rowTotal = dataGridPrestacaoContas.Rows[index];
                        rowTotal.DefaultCellStyle.BackColor = Color.DarkGray;
                        rowTotal.DefaultCellStyle.ForeColor = Color.Black; // ou Color.White, conforme preferir
                        rowTotal.DefaultCellStyle.Font = new Font(dataGridPrestacaoContas.Font, FontStyle.Bold);
                    }
                    else
                    {
                        // Linhas de dados (prestações concluídas)
                        dataGridPrestacaoContas.Rows.Add(
                            row["Nome"],
                            row["QuantidadeEntregue"],
                            row["NomeProduto"],
                            Convert.ToDouble(row["Preco"]).ToString("N2"),
                            row["QuantidadeVendida"],
                            row["QuantidadeDevolvida"],
                            Convert.ToDouble(row["ValorRecebido"]).ToString("N2"),
                            Convert.ToDouble(row["Comissao"]).ToString("N2"),
                            row["DataPrestacao"] != DBNull.Value ? Convert.ToDateTime(row["DataPrestacao"]).ToString("dd/MM/yyyy") : "",
                            row["EntregaID"],
                            row["PrestacaoID"],
                            row["VendedorID"]
                        );
                    }
                }

                // Atualizar o total de registros (exclui a linha de totais)
                int totalRegistros = dataGridPrestacaoContas.Rows.Count - 1;
                lblTotalRegistros.Text = $"Total de Registros: {totalRegistros}"; // Assumindo que lblTotalRegistros existe

                LogUtil.WriteLog($"PesquisarPrestaçõesPorVendedor concluído com {dt.Rows.Count} linhas.");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog($"Erro em PesquisarPrestaçõesPorVendedor: {ex.Message}");
                MessageBox.Show($"Erro ao pesquisar prestações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para estilizar a linha de totais
        private void DataGridPrestacaoContas_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //if (dataGridPrestacaoContas.Rows.Count > 1) // Garantir que há mais de uma linha
            //{
            //    int ultimaLinha = dataGridPrestacaoContas.Rows.Count - 1;
            //    if (dataGridPrestacaoContas.Rows[ultimaLinha].Cells["Nome"].Value?.ToString() == "TOTAIS")
            //    {
            //        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.BackColor = Color.DarkGray;
            //        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.ForeColor = Color.Black;
            //        dataGridPrestacaoContas.Rows[ultimaLinha].DefaultCellStyle.Font = new Font(dataGridPrestacaoContas.Font, FontStyle.Bold);
            //    }
            //}
        }







        private void CarregaDados()
        {
            FrmManipularPrestacaoDeContas formPrestacaoContas = new FrmManipularPrestacaoDeContas(StatusOperacao);

            if (StatusOperacao == "ALTERAR" || StatusOperacao == "EXCLUSÃO" || StatusOperacao == "ESTORNAR")
            {
                try
                {
                    if (dataGridPrestacaoContas.Rows.Count == 0 || dataGridPrestacaoContas.CurrentRow == null)
                    {
                        MessageBox.Show("Nenhuma prestação selecionada. Selecione uma linha na tabela.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    PrestacaoDeContasDAL objetoDAL = new PrestacaoDeContasDAL();
                    DataTable dt = objetoDAL.listaEntregasConcluidas();
                    int selectedIndex = dataGridPrestacaoContas.CurrentRow.Index;

                    if (dt.Rows[selectedIndex]["Nome"].ToString() == "TOTAIS")
                    {
                        MessageBox.Show("A linha de totais não pode ser editada. Selecione uma prestação específica.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dt.Columns.Contains("VendedorID") && dt.Rows[selectedIndex]["VendedorID"] != DBNull.Value)
                    {
                        int vendedorID = Convert.ToInt32(dt.Rows[selectedIndex]["VendedorID"]);
                        formPrestacaoContas.VendedorID = vendedorID;

                        if (StatusOperacao == "ALTERAR")
                        {
                            formPrestacaoContas.Text = "Alterar Prestações de Contas do Vendedor";
                            formPrestacaoContas.btnAlterarOuExcluir.Text = "Alterar";
                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(245, 166, 35); // Laranja
                            formPrestacaoContas.btnAlterarOuExcluir.ForeColor = Color.White;
                            formPrestacaoContas.pnFaixaDestaque.BackColor = Color.FromArgb(245, 166, 35); // Laranja
                            formPrestacaoContas.lblTituloFaixa.Text = "Alterar Prestações de Contas";
                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(245, 166, 35); // Laranja
                            formPrestacaoContas.btnAlterarOuExcluir.Image = Properties.Resources.tarefa32;

                        }
                        else if (StatusOperacao == "EXCLUSÃO")
                        {
                            formPrestacaoContas.Text = "Excluir Prestações de Contas do Vendedor";
                            formPrestacaoContas.btnAlterarOuExcluir.Text = "Excluir";
                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(255, 0, 0); // Vermelho
                            formPrestacaoContas.btnAlterarOuExcluir.ForeColor = Color.White;
                            formPrestacaoContas.dgvPrestacaoDeContas.ReadOnly = true;
                            formPrestacaoContas.pnFaixaDestaque.BackColor = Color.FromArgb(255, 0, 0); // Vermelho
                            formPrestacaoContas.lblTituloFaixa.Text = "Excluir Prestações de Contas";
                            formPrestacaoContas.btnAlterarOuExcluir.Image = Properties.Resources.excluir32_Branco;

                            // Desabilitar botões de ação
                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(255, 0, 0); // Vermelho
                            formPrestacaoContas.txtTotalComissao.Enabled = false;
                            formPrestacaoContas.txtTotalRecebido.Enabled = false;
                            formPrestacaoContas.txtTotalComissao.Enabled = false;
                            formPrestacaoContas.txtTotalDevolvida.Enabled = false;
                            formPrestacaoContas.txtTotalVendida.Enabled = false;
                            formPrestacaoContas.txtTotalEntregue.Enabled = false;

                        }
                        else if (StatusOperacao == "ESTORNAR")
                        {
                            formPrestacaoContas.Text = "Estornar Prestações de Contas do Vendedor";
                            formPrestacaoContas.btnAlterarOuExcluir.Text = "Estornar";
                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(255, 215, 0); // Amarelo
                            formPrestacaoContas.btnAlterarOuExcluir.ForeColor = Color.Black;
                            formPrestacaoContas.dgvPrestacaoDeContas.ReadOnly = true;
                            formPrestacaoContas.pnFaixaDestaque.BackColor = Color.FromArgb(255, 215, 0); // Amarelo
                            formPrestacaoContas.lblTituloFaixa.Text = "Estornar Prestações de Contas";
                            formPrestacaoContas.lblTituloFaixa.ForeColor = Color.FromArgb(8, 142, 255); // Amarelo

                            formPrestacaoContas.btnAlterarOuExcluir.BackColor = Color.FromArgb(255, 215, 0); // Amarelo
                            formPrestacaoContas.txtTotalComissao.Enabled = false;
                            formPrestacaoContas.txtTotalRecebido.Enabled = false;
                            formPrestacaoContas.txtTotalComissao.Enabled = false;
                            formPrestacaoContas.txtTotalDevolvida.Enabled = false;
                            formPrestacaoContas.txtTotalVendida.Enabled = false;
                            formPrestacaoContas.txtTotalEntregue.Enabled = false;
                            formPrestacaoContas.btnAlterarOuExcluir.Image = Properties.Resources.restituicao32;
                        }


                        formPrestacaoContas.ShowDialog();
                        Listar();
                    }
                    else
                    {
                        MessageBox.Show("A coluna VendedorID não foi encontrada ou está vazia na linha selecionada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Operação inválida para este formulário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExportarParaExcel()
        {
            try
            {
                // Verificar se o grid tem linhas
                if (dataGridPrestacaoContas.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Criar um DataTable manualmente a partir das linhas do DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("Nome", typeof(string));
                dt.Columns.Add("QuantidadeEntregue", typeof(long));
                dt.Columns.Add("NomeProduto", typeof(string));
                dt.Columns.Add("Preco", typeof(double));
                dt.Columns.Add("QuantidadeVendida", typeof(long));
                dt.Columns.Add("QuantidadeDevolvida", typeof(long));
                dt.Columns.Add("ValorRecebido", typeof(double));
                dt.Columns.Add("Comissao", typeof(double));
                dt.Columns.Add("DataPrestacao", typeof(string));
                dt.Columns.Add("EntregaID", typeof(int));
                dt.Columns.Add("PrestacaoID", typeof(int));
                dt.Columns.Add("VendedorID", typeof(int));

                // Preencher o DataTable com os dados do DataGridView
                foreach (DataGridViewRow row in dataGridPrestacaoContas.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["Nome"] = row.Cells["Nome"].Value?.ToString() ?? "";
                    dr["QuantidadeEntregue"] = row.Cells["QuantidadeEntregue"].Value != null && row.Cells["QuantidadeEntregue"].Value != DBNull.Value ? Convert.ToInt64(row.Cells["QuantidadeEntregue"].Value) : 0;
                    dr["NomeProduto"] = row.Cells["NomeProduto"].Value?.ToString() ?? "";
                    dr["Preco"] = row.Cells["Preco"].Value != null && row.Cells["Preco"].Value != DBNull.Value ? Convert.ToDouble(row.Cells["Preco"].Value.ToString().Replace("R$", "").Trim()) : 0.0;
                    dr["QuantidadeVendida"] = row.Cells["QuantidadeVendida"].Value != null && row.Cells["QuantidadeVendida"].Value != DBNull.Value ? Convert.ToInt64(row.Cells["QuantidadeVendida"].Value) : 0;
                    dr["QuantidadeDevolvida"] = row.Cells["QuantidadeDevolvida"].Value != null && row.Cells["QuantidadeDevolvida"].Value != DBNull.Value ? Convert.ToInt64(row.Cells["QuantidadeDevolvida"].Value) : 0;
                    dr["ValorRecebido"] = row.Cells["ValorRecebido"].Value != null && row.Cells["ValorRecebido"].Value != DBNull.Value ? Convert.ToDouble(row.Cells["ValorRecebido"].Value.ToString().Replace("R$", "").Trim()) : 0.0;
                    dr["Comissao"] = row.Cells["Comissao"].Value != null && row.Cells["Comissao"].Value != DBNull.Value ? Convert.ToDouble(row.Cells["Comissao"].Value.ToString().Replace("R$", "").Trim()) : 0.0;
                    dr["DataPrestacao"] = row.Cells["DataPrestacao"].Value?.ToString() ?? "";

                    // Tratar EntregaID, PrestacaoID e VendedorID explicitamente
                    if (row.Cells["EntregaID"].Value != null && row.Cells["EntregaID"].Value != DBNull.Value)
                        dr["EntregaID"] = Convert.ToInt32(row.Cells["EntregaID"].Value);
                    else
                        dr["EntregaID"] = DBNull.Value;

                    if (row.Cells["PrestacaoID"].Value != null && row.Cells["PrestacaoID"].Value != DBNull.Value)
                        dr["PrestacaoID"] = Convert.ToInt32(row.Cells["PrestacaoID"].Value);
                    else
                        dr["PrestacaoID"] = DBNull.Value;

                    if (row.Cells["VendedorID"].Value != null && row.Cells["VendedorID"].Value != DBNull.Value)
                        dr["VendedorID"] = Convert.ToInt32(row.Cells["VendedorID"].Value);
                    else
                        dr["VendedorID"] = DBNull.Value;

                    dt.Rows.Add(dr);
                }

                // Exportar o DataTable para Excel
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    sfd.FileName = "Relacao_De_Entregas_Com_Prestacao_De_Contas_Concluidas_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
                LogUtil.WriteLog($"Erro ao exportar para Excel: {ex.Message}");
                MessageBox.Show("Erro ao exportar para Excel: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void HabilitarTimer(bool habilitar)
        {
            timer1.Enabled = habilitar;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Listar();
            timer1.Enabled = false;
        }

        private void FrmManutencaoPrestacaoDeContas_KeyDown(object sender, KeyEventArgs e)
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
          

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            StatusOperacao = "ALTERAR";
            CarregaDados();
        }

        private void btnEstornar_Click(object sender, EventArgs e)
        {
            StatusOperacao = "ESTORNAR"; // Reverter a prestação
            CarregaDados();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            StatusOperacao = "EXCLUSÃO"; // Excluir permanentemente
            CarregaDados();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarParaExcel();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void txtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string textoPesquisa = txtPesquisa.Text.Trim();
            LogUtil.WriteLog($"txtPesquisa_TextChanged disparado com texto: {textoPesquisa}");
            Listar(textoPesquisa);
        }

        private void FrmManutencaoPrestacaoDeContasConcluidas_Load(object sender, EventArgs e)
        {
            LogUtil.WriteLog("FrmManutencaoPrestacaoDeContasConcluidas_Load iniciado.");
            ConfigurarColunasDataGridView(); // Configura as colunas primeiro
            Listar(); // Carrega os dados depois
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LogUtil.WriteLog("FrmManutencaoPrestacaoDeContasConcluidas_Load iniciado.");
            ConfigurarColunasDataGridView(); // Configura as colunas primeiro
            Listar(); // Carrega os dados depois
                      // Se houver um lblTotalRegistros, atualize aqui:
                      // Utilitario.AtualizarTotalRegistros(lblTotalRegistros, dataGridPrestacaoContas);
        }
    }
}
