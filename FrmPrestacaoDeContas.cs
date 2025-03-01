using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using static ComissPro.Model;

namespace ComissPro
{
    public partial class FrmPrestacaoDeContas : KryptonForm
    {
        private EntregasModel entregaSelecionada;
        public string StatusOperacao { get; set; }
        private string QueryPrestacao = "SELECT MAX(PrestacaoID) FROM PrestacaoContas";
        public int PrestacaoID { get; set; }
        public int EntregaID { get; set; }
        public int VendedorID { get; set; }
        public int ProdutoID { get; set; }
        public int QuantidadeEntregue { get; set; }
        public DateTime DataEntrega { get; set; }
        private double percentualComissao = 0;

        public FrmPrestacaoDeContas(string statusOperacao)
        {
            this.StatusOperacao = statusOperacao;
            InitializeComponent();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        private void CarregarComboEntregas()
        {
            try
            {
                var entregas = new EntregasDal().CarregarEntregasNaoPrestadas();
                if (entregas == null || entregas.Count == 0)
                {
                    MessageBox.Show("Nenhuma entrega pendente encontrada.");
                    return;
                }

                // Limpar e configurar o ComboBox
                cmbEntregasPendentes.DataSource = null;
                cmbEntregasPendentes.Items.Clear();
                cmbEntregasPendentes.DisplayMember = "ToString"; // Define antes do DataSource
                cmbEntregasPendentes.ValueMember = "EntregaID";
                cmbEntregasPendentes.DataSource = entregas;

                // Selecionar o primeiro item
                if (entregas.Count > 0)
                {
                    cmbEntregasPendentes.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar entregas: {ex.Message}");
            }
        }

        private List<EntregaComboItem> CarregarEntregasNaoPrestadas()
        {
            List<EntregaComboItem> entregas = new List<EntregaComboItem>();
            string query = @"SELECT EntregaID, VendedorID, ProdutoID, QuantidadeEntregue, DataEntrega 
                            FROM Entregas WHERE PrestacaoRealizada = 0";

            using (var conn = Conexao.Conex())
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entregas.Add(new EntregaComboItem
                            {
                                EntregaID = reader.GetInt32(0),
                                VendedorID = reader.GetInt64(1),
                                ProdutoID = reader.GetInt64(2),
                                QuantidadeEntregue = reader.GetInt64(3),
                                DataEntrega = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return entregas;
        }

        private double BuscarPrecoProduto(long produtoID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT Preco FROM Produtos WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", produtoID);
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToDouble(resultado) : 0.0;
                }
            }
        }

        private string BuscarNomeVendedor(long vendedorID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT Nome FROM Vendedores WHERE VendedorID = @VendedorID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", vendedorID);
                    return cmd.ExecuteScalar()?.ToString() ?? "Vendedor não encontrado";
                }
            }
        }

        private string BuscarNomeProduto(long produtoID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT NomeProduto FROM Produtos WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", produtoID);
                    return cmd.ExecuteScalar()?.ToString() ?? "Produto não encontrado";
                }
            }
        }
        private double BuscarPercentualComissao(long vendedorID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT Comissao FROM Vendedores WHERE VendedorID = @VendedorID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendedorID", vendedorID);
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToDouble(resultado) : 10; // 10% padrão
                }
            }
        }
        public void SalvarRegistro()
        {
            try
            {
                Model.PrestacaoContasModel objetoModel = new Model.PrestacaoContasModel();

                objetoModel.PrestacaoID = PrestacaoID;
                objetoModel.EntregaID = EntregaID;
                objetoModel.QuantidadeVendida = int.Parse(txtQuantidadeVendida.Text);
                objetoModel.QuantidadeDevolvida = int.Parse(txtQtdDevolvida.Text);
                objetoModel.ValorRecebido = double.Parse(txtValorRecebido.Text);
                objetoModel.Comissao = double.Parse(txtValorComissao.Text);
                objetoModel.DataPrestacao = dtpDataPrestacaoContas.Value;

                PrestacaoDeContasBLL objetoBll = new PrestacaoDeContasBLL();

                objetoBll.Salvar(objetoModel);
                MessageBox.Show("REGISTRO gravado com sucesso! ", "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Utilitario.LimpaCampo(this);
            }
            catch (OverflowException ov)
            {
                MessageBox.Show("Overfow Exeção deu erro! " + ov);
            }
            catch (Win32Exception erro)
            {
                MessageBox.Show("Win32 Win32!!! \n" + erro);
            }
        }
        public void AlterarRegistro()
        {
            try
            {
                Model.PrestacaoContasModel objetoModel = new Model.PrestacaoContasModel();

                objetoModel.PrestacaoID = PrestacaoID;
                objetoModel.EntregaID = EntregaID;
                objetoModel.QuantidadeVendida = int.Parse(txtQuantidadeVendida.Text);
                objetoModel.QuantidadeDevolvida = int.Parse(txtQtdDevolvida.Text);
                objetoModel.ValorRecebido = double.Parse(txtValorRecebido.Text);
                objetoModel.Comissao = double.Parse(txtValorComissao.Text);
                objetoModel.DataPrestacao = dtpDataPrestacaoContas.Value;


                PrestacaoDeContasBLL objetoBll = new PrestacaoDeContasBLL();
                objetoBll.Alterar(objetoModel);

                MessageBox.Show("Registro Alterado com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ((FrmManutVendedor)Application.OpenForms["FrmManutVendedor"]).HabilitarTimer(true);// Habilita Timer do outro form Obs: O timer no outro form executa um Método.    
                Utilitario.LimpaCampo(this);
                this.Close();
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao Alterar o registro!!! " + erro);
            }
        }
        public void ExcluirRegistro()
        {
            try
            {
                Model.VendedorMODEL objetoModel = new Model.VendedorMODEL();

                objetoModel.VendedorID = VendedorID;
                VendedorBLL objetoBll = new VendedorBLL();

                objetoBll.Excluir(objetoModel);
                MessageBox.Show("Registro Excluído com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Limpa os campos
                Utilitario.LimpaCampo(this);
                this.Close();
                var frmManutPrestacaodeContas = Application.OpenForms["FrmManutPrestacaoDeContas"] as FrmManutencaoPrestacaoDeContas;

                if (frmManutPrestacaodeContas != null)
                {
                    frmManutPrestacaodeContas.HabilitarTimer(true);
                }
                else
                {
                    MessageBox.Show("FrmManutPrestacaoDeContas não está aberto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao Excluir o registro: " + erro.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (entregaSelecionada == null)
            {
                MessageBox.Show("Selecione uma entrega antes de salvar!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuantidadeVendida.Text) ||
                string.IsNullOrWhiteSpace(txtQtdDevolvida.Text) ||
                string.IsNullOrWhiteSpace(txtValorRecebido.Text) ||
                string.IsNullOrWhiteSpace(txtValorComissao.Text))
            {
                MessageBox.Show("Preencha todos os campos antes de salvar!");
                return;
            }

            var prestacao = new PrestacaoContasModel
            {
                EntregaID = entregaSelecionada.EntregaID,
                QuantidadeVendida = int.Parse(txtQuantidadeVendida.Text),
                QuantidadeDevolvida = int.Parse(txtQtdDevolvida.Text),
                ValorRecebido = double.Parse(txtValorRecebido.Text, NumberStyles.Currency),
                Comissao = double.Parse(txtValorComissao.Text, NumberStyles.Currency),
                DataPrestacao = dtpDataPrestacaoContas.Value
            };

            var entregasDAL = new PrestacaoDeContasDAL();
            entregasDAL.SalvarPrestacaoDeContas(prestacao);
            MessageBox.Show("Prestação de contas salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CarregarComboEntregas();
            
            ((FrmManutençãodeEntregaBilhetes)Application.OpenForms["FrmManutençãodeEntregaBilhetes"]).HabilitarTimer(true);
            Utilitario.LimpaCampoKrypton(this);
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }            
        private void FrmPrestacaoDeContas_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryPrestacao);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                //string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                PrestacaoID = NovoCodigo;
                
            }
            CarregarComboEntregas();
        }

        private void cmbEntregasPendentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEntregasPendentes.SelectedItem != null)
            {
                entregaSelecionada = (EntregasModel)cmbEntregasPendentes.SelectedItem;

                txtNomeVendedor.Text = entregaSelecionada.NomeVendedor;
                txtNomeProduto.Text = entregaSelecionada.NomeProduto;
                txtQuantidadeEntregue.Text = entregaSelecionada.QuantidadeEntregue.ToString();
                EntregaID = entregaSelecionada.EntregaID;
                txtPrecoUnit.Text = entregaSelecionada.Preco.ToString("C");
                txtTotal.Text = entregaSelecionada.Total.ToString("C");
                dtpDataEntrega.Value = entregaSelecionada.DataEntrega.Value;

                percentualComissao = entregaSelecionada.Comissao;
                txtPercentualComissao.Text = percentualComissao.ToString("F2") + "%";

                txtQtdDevolvida.Text = "0";
                txtQuantidadeVendida.Text = entregaSelecionada.QuantidadeEntregue.ToString();
                txtValorRecebido.Text = "";
                txtValorComissao.Text = "";
            }
        }
        private void txtQtdDevolvida_Leave(object sender, EventArgs e)
        {
            if (entregaSelecionada == null)
            {
                MessageBox.Show("Selecione uma entrega primeiro!");
                txtQtdDevolvida.Text = "0";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQtdDevolvida.Text) || !int.TryParse(txtQtdDevolvida.Text, out int devolvida))
            {
                MessageBox.Show("Digite uma quantidade válida!");
                txtQtdDevolvida.Text = "0";
                txtQuantidadeVendida.Text = entregaSelecionada.QuantidadeEntregue.ToString();
                txtValorRecebido.Text = "";
                txtValorComissao.Text = "";
                return;
            }

            if (devolvida < 0)
            {
                MessageBox.Show("A quantidade devolvida não pode ser negativa!");
                txtQtdDevolvida.Text = "0";
                devolvida = 0;
            }

            int entregue = int.Parse(txtQuantidadeEntregue.Text);
            if (devolvida <= entregue)
            {
                int vendida = entregue - devolvida;
                txtQuantidadeVendida.Text = vendida.ToString();

                double precoProduto = entregaSelecionada.Preco; // Já está no modelo
                double valorRecebido = vendida * precoProduto;
                txtValorRecebido.Text = valorRecebido.ToString("C");

                double comissao = valorRecebido * (percentualComissao / 100);
                txtValorComissao.Text = comissao.ToString("C");
            }
            else
            {
                MessageBox.Show("Quantidade devolvida não pode ser maior que a entregue!");
                txtQtdDevolvida.Text = "0";
                txtQuantidadeVendida.Text = entregaSelecionada.QuantidadeEntregue.ToString();
                txtValorRecebido.Text = "";
                txtValorComissao.Text = "";
            }
        }

        private void FrmPrestacaoDeContas_KeyDown(object sender, KeyEventArgs e)
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
        private void RecalcularComissao()
        {
            if (!string.IsNullOrWhiteSpace(txtValorRecebido.Text))
            {
                double valorRecebido = double.Parse(txtValorRecebido.Text, NumberStyles.Currency);
                double comissao = valorRecebido * (percentualComissao / 100); // Divide por 100 para cálculo
                txtValorComissao.Text = comissao.ToString("N2");
            }
        }
        private void txtPercentualComissao_Leave(object sender, EventArgs e)
        {
            if (entregaSelecionada == null)
            {
                return;
            }

            string texto = txtPercentualComissao.Text.Replace("%", "").Trim();
            if (double.TryParse(texto, out double novoPercentual) && novoPercentual >= 0 && novoPercentual <= 100)
            {
                percentualComissao = novoPercentual;
                RecalcularComissao();
            }
            else
            {
                MessageBox.Show("Digite um percentual válido entre 0 e 100!");
                txtPercentualComissao.Text = percentualComissao.ToString("F2") + "%";
            }
        }
    }
    public class EntregaComboItem
    {
        public int EntregaID { get; set; }
        public long VendedorID { get; set; }
        public long ProdutoID { get; set; }
        public long QuantidadeEntregue { get; set; }
        public DateTime DataEntrega { get; set; }
        public double Comissao { get; set; }
        public string NomeVendedor { get; set; } // Adicionado para o ToString

        public override string ToString()
        {
            return $"{NomeVendedor} - Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
        }
    }

}
