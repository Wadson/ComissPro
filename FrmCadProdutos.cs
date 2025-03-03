using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmCadProdutos : KryptonForm
    {
        private string StatusOperacao;
        private string QueryProduto = "SELECT MAX(ProdutoID) FROM Produtos";
        private int ProdutoID;
        public FrmCadProdutos(string statusOperacao)
        {
            this.StatusOperacao = statusOperacao;
            InitializeComponent();
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }

        public void SalvarRegistro()
        {
            try
            {
                Model.ProdutoMODEL objetoModel = new Model.ProdutoMODEL();

                objetoModel.ProdutoID = string.IsNullOrEmpty(txtProdutoID.Text) ? 0 : Convert.ToInt32(txtProdutoID.Text);
                objetoModel.NomeProduto = txtNome.Text;
                objetoModel.Unidade = cmbUnidade.Text;
                objetoModel.Preco = Double.Parse(txtPreco.Text);

                ProdutoBLL objetoBll = new ProdutoBLL();
                objetoBll.Salvar(objetoModel);

                MessageBox.Show("REGISTRO gravado com sucesso!", "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Utilitario.LimpaCampo(this);
               
                //((FrmManutProduto)Application.OpenForms["FrmManutProduto"]).HabilitarTimer(true);

                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutProduto"] as FrmManutProduto;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
                }
            }
            catch (OverflowException ov)
            {
                MessageBox.Show("Overflow Exceção deu erro! " + ov);
            }
            catch (Win32Exception erro)
            {
                MessageBox.Show("Win32 Win32!!! \n" + erro);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar registro: " + ex.Message); // Exibirá a mensagem de duplicata aqui
            }
        }

        public void AlterarRegistro()
        {
            try
            {
                Model.ProdutoMODEL objetoModel = new Model.ProdutoMODEL();

                objetoModel.ProdutoID = Convert.ToInt32(txtProdutoID.Text);
                objetoModel.NomeProduto = txtNome.Text;
                objetoModel.Preco = double.Parse(txtPreco.Text);
                objetoModel.Unidade = cmbUnidade.Text;


                ProdutoBLL objetoBll = new ProdutoBLL();
                objetoBll.Alterar(objetoModel);

                MessageBox.Show("Registro Alterado com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutProduto"] as FrmManutProduto;
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
                Model.ProdutoMODEL objetoModel = new Model.ProdutoMODEL();

                objetoModel.ProdutoID = Convert.ToInt32(txtProdutoID.Text.Trim());
                ProdutoBLL objetoBll = new ProdutoBLL();

                objetoBll.Excluir(objetoModel);
                MessageBox.Show("Registro Excluído com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Limpa os campos
                Utilitario.LimpaCampo(this);
                this.Close();

                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutProduto"] as FrmManutProduto;
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


        //
       

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                AlterarRegistro();
            }
            if (StatusOperacao == "NOVO")
            {
                SalvarRegistro();
                txtNome.Focus();

                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryProduto);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                ProdutoID = NovoCodigo;
                txtProdutoID.Text = numeroComZeros;

                ((FrmManutProduto)Application.OpenForms["FrmManutProduto"]).HabilitarTimer(true);

            }
            if (StatusOperacao == "EXCLUSÃO")
            {
                if (MessageBox.Show("Deseja Excluir? \n\n O Usuário: " + txtNome.Text + " ??? ", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExcluirRegistro();
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            Utilitario.LimpaCampo(this);

            int NovoCodigo = Utilitario.GerarProximoCodigo(QueryProduto);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
            string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
            ProdutoID = NovoCodigo;
            txtProdutoID.Text = numeroComZeros;
        }

        private void FrmCadProdutos_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryProduto);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                ProdutoID = NovoCodigo;
                txtProdutoID.Text = numeroComZeros;
            }
        }

        private void txtPreco_Leave(object sender, EventArgs e)
        {
            Utilitario.FormatTextBoxToCurrencyKrypton(txtPreco);
        }

        private void FrmCadProdutos_KeyDown(object sender, KeyEventArgs e)
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
