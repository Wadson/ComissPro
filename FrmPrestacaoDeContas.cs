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
        private EntregasModel entregaSelecionada; // Variável de instância
        private string StatusOperacao;
        private string QueryPrestacao = "SELECT MAX(PrestacaoID) FROM PrestacaoContas";
        private int PrestacaoID;
        private int EntregaID;
        private int VendedorID;
        public FrmPrestacaoDeContas()
        {
            InitializeComponent();
        }
        public void SalvarRegistro()
        {
            try
            {
                Model.PrestacaoContasModel objetoModel = new Model.PrestacaoContasModel();

                objetoModel.PrestacaoID = Convert.ToInt32(txtPrestacaoID.Text);
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

                objetoModel.PrestacaoID = Convert.ToInt32(txtPrestacaoID.Text);
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
                var frmManutPrestacaodeContas = Application.OpenForms["FrmManutPrestacaoDeContas"] as FrmManutPrestacaoDeContas;

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
        private void CarregarComboEntregas()
        {
            // Declara e inicializa a lista de entregas com a consulta
            var entregas = CarregarEntregasNaoPrestadas();

            // Associa ao ComboBox
            cmbEntregasPendentes.DataSource = entregas;
            cmbEntregasPendentes.DisplayMember = "ToString"; // Usa o método ToString personalizado
            cmbEntregasPendentes.ValueMember = "EntregaID";  // Define o valor retornado
        }
        public class EntregaComboItem
        {
            public int EntregaID { get; set; }
            public long VendedorID { get; set; }
            public long ProdutoID { get; set; }
            public long QuantidadeEntregue { get; set; }
            public DateTime DataEntrega { get; set; }

            public override string ToString()
            {
                return $"Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
            }
        }


        // Método auxiliar para buscar entregas não prestadas
        private List<EntregaComboItem> CarregarEntregasNaoPrestadas()
        {
            string connectionString = "Data Source=seu_banco.db;Version=3;";
            List<EntregaComboItem> entregas = new List<EntregaComboItem>();
            string query = @"SELECT EntregaID, VendedorID, ProdutoID, QuantidadeEntregue, DataEntrega 
                     FROM Entregas 
                     WHERE PrestacaoRealizada = 0"; // Filtra entregas não prestadas

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
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
                                EntregaID = reader.GetInt32(0),           // EntregaID
                                VendedorID = reader.GetInt64(1),          // VendedorID
                                ProdutoID = reader.GetInt64(2),           // ProdutoID
                                QuantidadeEntregue = reader.GetInt64(3),  // QuantidadeEntregue
                                DataEntrega = reader.GetDateTime(4)       // DataEntrega
                            });
                        }
                    }
                }
            }
            return entregas;
        }

        // Métodos auxiliares (exemplo)
        private string BuscarNomeVendedor(long vendedorID)
        {
            using (var conn = Conexao.Conex())
            {
                conn.Open();
                string query = "SELECT Nome FROM Vendedor WHERE VendedorID = @VendedorID";
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
                string query = "SELECT NomeProduto FROM Produto WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", produtoID);
                    return cmd.ExecuteScalar()?.ToString() ?? "Produto não encontrado";
                }
            }
        }
        private double BuscarPrecoProduto(long produtoID)
        {
            string connectionString = "Data Source=seu_banco.db;Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Preco FROM Produto WHERE ProdutoID = @ProdutoID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProdutoID", produtoID);
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToDouble(resultado) : 0.0; // Retorna 0 se não encontrar
                }
            }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            var prestacao = new PrestacaoContasModel
            {
                EntregaID = entregaSelecionada.EntregaID,
                QuantidadeVendida = int.Parse(txtQuantidadeVendida.Text),
                QuantidadeDevolvida = int.Parse(txtQtdDevolvida.Text),
                ValorRecebido = double.Parse(txtValorRecebido.Text, NumberStyles.Currency),
                Comissao = double.Parse(txtComissao.Text, NumberStyles.Currency),
                DataPrestacao = dtpDataPrestacaoContas.Value
            };

            // Chama o método da DAL
            var prestacaoContas = new PrestacaoDeContasDAL();
            prestacaoContas.SalvarPrestacaoDeContas(prestacao);

            // Atualiza o ComboBox
            CarregarComboEntregas();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {

        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtQtdDevolvida_TextChanged(object sender, EventArgs e)
        {
            if (entregaSelecionada == null)
            {
                MessageBox.Show("Selecione uma entrega primeiro!");
                return;
            }

            if (int.TryParse(txtQtdDevolvida.Text, out int devolvida) &&
                int.TryParse(txtQuantidadeEntregue.Text, out int entregue))
            {
                if (devolvida <= entregue)
                {
                    int vendida = entregue - devolvida;
                    txtQuantidadeVendida.Text = vendida.ToString();

                    double precoProduto = BuscarPrecoProduto(entregaSelecionada.ProdutoID);
                    double valorRecebido = vendida * precoProduto;
                    txtValorRecebido.Text = valorRecebido.ToString("C");

                    double comissao = valorRecebido * 0.10; // 10% configurável
                    txtComissao.Text = comissao.ToString("C");
                }
                else
                {
                    MessageBox.Show("Quantidade devolvida não pode ser maior que a entregue!");
                }
            }
        }

        private void cmbEntregasPendentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            entregaSelecionada = (EntregasModel)cmbEntregasPendentes.SelectedItem;
            txtVendedor.Text = BuscarNomeVendedor(entregaSelecionada.VendedorID);
            txtProduto.Text = BuscarNomeProduto(entregaSelecionada.ProdutoID);
            txtQuantidadeEntregue.Text = entregaSelecionada.QuantidadeEntregue.ToString();
        }

        private void FrmPrestacaoDeContas_Load(object sender, EventArgs e)
        {
            CarregarComboEntregas();
        }
    }
}
