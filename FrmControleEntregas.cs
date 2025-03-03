using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;


namespace ComissPro
{
    public partial class FrmControleEntregas : KryptonForm
    {
        public bool bloqueiaEventosTextChanged = false;

        private bool bloqueiaPesquisa = false;
        public int ProdutoID {get; set;}
        public int VendedorID { get; set; }
        public string vendedorSelecionado { get; set; }
        private int nextItemVendaID;
        private string StatusOperacao;
        private string QueryControleEntrega = "SELECT MAX(EntregaID) FROM Entregas";
        private int EntregaID;
        

        public FrmControleEntregas(string statusOperacao)
        {
            InitializeComponent();
            this.StatusOperacao = statusOperacao;
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        public void CalcularSubtotal()
        {
            try
            {
                if (decimal.TryParse(txtQuantidade.Text, out decimal quantidade) &&
                    decimal.TryParse(txtPrecoUnit.Text, out decimal precoUnitario))                    
                {
                    decimal subtotal = (quantidade * precoUnitario);
                    txtTotal.Text = subtotal.ToString("N2");
                    ToMoney(txtPrecoUnit);
                    ToMoney(txtTotal);
                }
                else
                {
                    txtTotal.Text = "0.00";
                    MessageBox.Show("Por favor, preencha todos os campos corretamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message);
            }
        }
        private void LocalizarProduto()
        {
            using (FrmLocalizarProduto frmpesquisarProduto = new FrmLocalizarProduto(this, txtNomeProduto.Text))
            {
                frmpesquisarProduto.Owner = this;
                frmpesquisarProduto.produtoSelecionado = txtNomeProduto.Text;
                frmpesquisarProduto.ShowDialog();
                frmpesquisarProduto.Text = "Localizar Produtos";

                if (!string.IsNullOrEmpty(frmpesquisarProduto.produtoSelecionado))
                {
                    txtNomeProduto.Text = frmpesquisarProduto.produtoSelecionado;
                }
            }
            // Ajustes adicionais
            ToMoney(txtPrecoUnit);
            ToMoney(txtTotal);
            txtQuantidade.Focus();
            CalcularSubtotal();
        }
        public void ToMoney(KryptonTextBox text, string format = "N")
        {
            if (decimal.TryParse(text.Text, out decimal value))
            {
                text.Text = value.ToString(format);
            }
            else
            {
                text.Text = "0,00";
            }
        }       
        private void AbrirFrmLocalizarVendedor()
        {
            // Desliga temporariamente o evento para evitar loop
            txtNomeVendedor.TextChanged -= txtNomeVendedor_TextChanged;

            using (FrmLocalicarVendedor frmlocalizarVendedor = new FrmLocalicarVendedor(this, txtNomeVendedor.Text))
            {
                frmlocalizarVendedor.Owner = this;
                frmlocalizarVendedor.vendedorSelecionado = txtNomeVendedor.Text;
                frmlocalizarVendedor.ShowDialog();
                frmlocalizarVendedor.Text = "Localizar Vendedor";

                if (!string.IsNullOrEmpty(frmlocalizarVendedor.vendedorSelecionado))
                {
                    txtNomeProduto.Text = frmlocalizarVendedor.vendedorSelecionado;
                }               
            }
        }
        public void SalvarRegistro()
        {
            if (!TrialManager.IsTrialActive())
            {
                MessageBox.Show("O período de avaliação expirou. Não é possível salvar ou editar registros.", "Trial Expirado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Model.EntregasModel objetoModel = new Model.EntregasModel();
                objetoModel.EntregaID = string.IsNullOrEmpty(txtEntregaID.Text) ? 0 : Convert.ToInt32(txtEntregaID.Text);
                objetoModel.VendedorID = VendedorID;
                objetoModel.ProdutoID = ProdutoID;
                objetoModel.QuantidadeEntregue = int.Parse(txtQuantidade.Text);
                objetoModel.DataEntrega = dtpDataEntregaBilhete.Value;

                EntregasBLL objetoBll = new EntregasBLL();
                objetoBll.Salvar(objetoModel);

                MessageBox.Show("Registro gravado com sucesso!", "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                var frmManutencao = Application.OpenForms["FrmManutençãodeEntregaBilhetes"] as FrmManutençãodeEntregaBilhetes;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar();
                }

                Utilitario.LimpaCampo(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar registro: " + ex.Message);
            }
        }

        //public void SalvarRegistro()
        //{
        //    try
        //    {
        //        Model.EntregasModel objetoModel = new Model.EntregasModel();

        //        objetoModel.EntregaID = string.IsNullOrEmpty(txtEntregaID.Text) ? 0 : Convert.ToInt32(txtEntregaID.Text);
        //        objetoModel.VendedorID = VendedorID;
        //        objetoModel.ProdutoID = ProdutoID;
        //        objetoModel.QuantidadeEntregue = int.Parse(txtQuantidade.Text);
        //        objetoModel.DataEntrega = dtpDataEntregaBilhete.Value;

        //        EntregasBLL objetoBll = new EntregasBLL();
        //        objetoBll.Salvar(objetoModel);

        //        MessageBox.Show("Registro gravado com sucesso!",
        //                        "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        //        // Tenta acessar o formulário e atualizar diretamente
        //        var frmManutencao = Application.OpenForms["FrmManutençãodeEntregaBilhetes"] as FrmManutençãodeEntregaBilhetes;
        //        if (frmManutencao != null)
        //        {
        //            frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
        //        }

        //        Utilitario.LimpaCampo(this);
        //    }
        //    catch (OverflowException ov)
        //    {
        //        MessageBox.Show("Overflow Exception deu erro! " + ov);
        //    }
        //    catch (Win32Exception erro)
        //    {
        //        MessageBox.Show("Win32 Exception!!! \n" + erro);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Erro ao salvar registro: " + ex.Message);
        //    }
        //}
        public void AlterarRegistro()
        {
            try
            {
                Model.EntregasModel objetoModel = new Model.EntregasModel();

                objetoModel.EntregaID = Convert.ToInt32(txtEntregaID.Text);
                objetoModel.VendedorID = VendedorID;
                objetoModel.ProdutoID = ProdutoID;
                objetoModel.QuantidadeEntregue = int.Parse(txtQuantidade.Text);
                objetoModel.DataEntrega = dtpDataEntregaBilhete.Value;

                EntregasBLL objetoBll = new EntregasBLL();
                objetoBll.Alterar(objetoModel);

                MessageBox.Show("Registro Alterado com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutençãodeEntregaBilhetes"] as FrmManutençãodeEntregaBilhetes;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
                }
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
                Model.EntregasModel objetoModel = new Model.EntregasModel();

                objetoModel.EntregaID = Convert.ToInt32(txtEntregaID.Text.Trim());
                EntregasBLL objetoBll = new EntregasBLL();

                objetoBll.Excluir(objetoModel);
                MessageBox.Show("Registro Excluído com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Limpa os campos
                Utilitario.LimpaCampo(this);
                this.Close();

                // Atualiza o DataGridView no FrmManutençãodeEntregaBilhetes
                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutençãodeEntregaBilhetes"] as FrmManutençãodeEntregaBilhetes;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao Excluir o registro: " + erro.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Log(string message)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now}: {message}\n");
        }
        private void FrmControleEntregas_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryControleEntrega);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                VendedorID = NovoCodigo;
                txtEntregaID.Text = numeroComZeros;
                txtNomeVendedor.Focus();
            }
        }

        private void cmbVendedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Suprime o som de bip padrão
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void cmbProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Suprime o som de bip padrão
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }       

        private void btnLocalizarVendedor_Click(object sender, EventArgs e)
        {
            AbrirFrmLocalizarVendedor();
        }

        private void btnLocalizarProduto_Click(object sender, EventArgs e)
        {
            LocalizarProduto();
        }
        
        private void txtQuantidade_Leave(object sender, EventArgs e)
        {
            CalcularSubtotal();
        }

        private void txtPrecoUnit_Leave(object sender, EventArgs e)
        {
            CalcularSubtotal();            
        }       

        private void txtNomeVendedor_TextChanged(object sender, EventArgs e)
        {
            if (bloqueiaPesquisa || bloqueiaEventosTextChanged || string.IsNullOrEmpty(txtNomeVendedor.Text))
                return;

            bloqueiaPesquisa = true;

            using (FrmLocalicarVendedor pesquisaVendedor = new FrmLocalicarVendedor(this, txtNomeVendedor.Text))
            {
                pesquisaVendedor.Owner = this;

                if (pesquisaVendedor.ShowDialog() == DialogResult.OK)
                {
                    txtNomeVendedor.Text = pesquisaVendedor.vendedorSelecionado;
                }
            }

            Task.Delay(100).ContinueWith(t =>
            {
                Invoke(new Action(() => bloqueiaPesquisa = false));
            });          
        }

        private void txtNomeProduto_TextChanged(object sender, EventArgs e)
        {
            if (bloqueiaPesquisa || bloqueiaEventosTextChanged || string.IsNullOrEmpty(txtNomeProduto.Text))
                return;

            bloqueiaPesquisa = true;

            using (FrmLocalizarProduto pesquisaProduto = new FrmLocalizarProduto(this, txtNomeProduto.Text))
            {
                pesquisaProduto.Owner = this;

                if (pesquisaProduto.ShowDialog() == DialogResult.OK)
                {
                    if (txtNomeProduto.Text != pesquisaProduto.produtoSelecionado)
                    {
                        txtNomeProduto.Text = pesquisaProduto.produtoSelecionado;
                        txtQuantidade.Focus();
                    }
                }
            }

            Task.Delay(100).ContinueWith(t =>
            {
                Invoke(new Action(() => bloqueiaPesquisa = false));
            });

            txtQuantidade.Select();
        }
      
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                AlterarRegistro();
            }
            if (StatusOperacao == "NOVO")
            {
                SalvarRegistro();
                txtNomeVendedor.Focus();

                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryControleEntrega);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                VendedorID = NovoCodigo;
                txtEntregaID.Text = numeroComZeros;
            }
            if (StatusOperacao == "EXCLUSÃO")
            {
                if (MessageBox.Show("Deseja Excluir? \n\n O Usuário: " + txtNomeVendedor.Text + " ??? ", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExcluirRegistro();
                }
            }
        }

        private void btnNovo_Click_1(object sender, EventArgs e)
        {
            Utilitario.LimpaCampo(this);

            int NovoCodigo = Utilitario.GerarProximoCodigo(QueryControleEntrega);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
            string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
            EntregaID = NovoCodigo;
            txtEntregaID.Text = numeroComZeros;
        }

        private void btnSair_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmControleEntregas_KeyDown(object sender, KeyEventArgs e)
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
