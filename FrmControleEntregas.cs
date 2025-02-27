using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Guna.UI2.WinForms;

namespace ComissPro
{
    public partial class FrmControleEntregas : ComissPro.FrmModelo
    {
        public int ProdutoID {get; set;}
        public FrmControleEntregas()
        {
            InitializeComponent();
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
            // Desliga temporariamente o evento para evitar loop
            txtNomeProduto.TextChanged -= txtNomeProduto_TextChanged;

            using (FrmPesquisarProduto frmpesquisarProduto = new FrmPesquisarProduto(this, txtNomeProduto.Text))
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

            // Religa o evento após modificar o texto
            txtNomeProduto.TextChanged += txtNomeProduto_TextChanged;

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
        private void FrmControleEntregas_Load(object sender, EventArgs e)
        {            
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
        private void AbrirFrmLocalizarVendedor()
        {
            // Desliga temporariamente o evento para evitar loop
            txtNomeVendedor.TextChanged -= txtNomeVendedor_TextChanged;

            using (frmlo frmLocalizarCliente = new FrmLocalizarCliente(this, clienteSelecionado))
            {
                frmLocalizarCliente.Owner = this;
                frmLocalizarCliente.ShowDialog();
                txtNomeCliente.Text = frmLocalizarCliente.ClienteSelecionado; // Define o nome do cliente retornado
            }

            // Religa o evento após modificar o texto
            txtNomeCliente.TextChanged += txtNomeCliente_TextChanged;
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void btnFinalizarVenda_Click(object sender, EventArgs e)
        {

        }

        private void btnNovo_Click(object sender, EventArgs e)
        {

        }

        private void btnLocalizarVendedor_Click(object sender, EventArgs e)
        {

        }

        private void btnLocalizarProduto_Click(object sender, EventArgs e)
        {
            LocalizarProduto();
        }

        private void txtNomeProduto_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQuantidade_Leave(object sender, EventArgs e)
        {
            CalcularSubtotal();
        }

        private void txtPrecoUnit_Leave(object sender, EventArgs e)
        {
            CalcularSubtotal();            
        }
    }
}
