using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Guna.UI2.WinForms;

namespace ComissPro
{
    public partial class FrmControleEntregas : ComissPro.FrmModelo
    {
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
                //// Desliga temporariamente o evento para evitar loop
                //txtNomeVendedor.TextChanged -= txtNomeVendedor_TextChanged;

                //using (FrmLocalicarVendedor frmLocalizarVendedor = new FrmLocalicarVendedor(this, vendedorSelecionado))
                //{
                //    frmLocalizarVendedor.Owner = this;
                //    frmLocalizarVendedor.ShowDialog();
                //    txtNomeVendedor.Text = frmLocalizarVendedor.vendedorSelecionado; // Define o nome do cliente retornado
                //}

                //// Religa o evento após modificar o texto
                //txtNomeVendedor.TextChanged += txtNomeVendedor_TextChanged;
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
       
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {

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
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {

        }
    }
}
