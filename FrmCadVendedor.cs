using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class FrmCadVendedor : KryptonForm
    {
        private string StatusOperacao;
        private string QueryVendedor = "SELECT MAX(VendedorID) FROM Vendedores";
        private int VendedorID;
        public FrmCadVendedor(string statusOperacao)
        {            
            InitializeComponent();
            txtTelefone.KeyPress += new KeyPressEventHandler(Utilitario.FormataTelefone);
            this.StatusOperacao = statusOperacao;
            // Utiliza a classe Utilitario para adicionar os efeitos de foco a todos os TextBoxes no formulário
            Utilitario.AdicionarEfeitoFocoEmTodos(this);
        }
        public void SalvarRegistro()
        {
            try
            {
                Model.VendedorMODEL objetoModel = new Model.VendedorMODEL();

                objetoModel.VendedorID = Convert.ToInt32(txtVendedorID.Text);
                objetoModel.Nome = txtNomeVendedor.Text;
                objetoModel.CPF = txtCpf.Text;
                objetoModel.Telefone = txtTelefone.Text;
                objetoModel.Comissao = double.Parse(txtPercentualComissao.Text);

                VendedorBLL objetoBll = new VendedorBLL();

                objetoBll.Salvar(objetoModel);
                MessageBox.Show("REGISTRO gravado com sucesso! ", "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Utilitario.LimpaCampo(this);
                ((FrmManutVendedor)Application.OpenForms["FrmManutVendedor"]).HabilitarTimer(true);
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
                Model.VendedorMODEL objetoModel = new Model.VendedorMODEL();

                objetoModel.VendedorID = Convert.ToInt32(txtVendedorID.Text);
                objetoModel.Nome = txtNomeVendedor.Text;
                objetoModel.CPF = txtCpf.Text;
                objetoModel.Telefone = txtTelefone.Text;
                objetoModel.Comissao = double.Parse(txtPercentualComissao.Text);


                VendedorBLL objetoBll = new VendedorBLL();
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

                objetoModel.VendedorID = Convert.ToInt32(txtVendedorID.Text.Trim());
                VendedorBLL objetoBll = new VendedorBLL();

                objetoBll.Excluir(objetoModel);
                MessageBox.Show("Registro Excluído com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Limpa os campos
                Utilitario.LimpaCampo(this);
                this.Close();
                var frmManutFornecedor = Application.OpenForms["FrmManutVendedor"] as FrmManutVendedor;

                if (frmManutFornecedor != null)
                {
                    frmManutFornecedor.HabilitarTimer(true);
                }
                else
                {
                    MessageBox.Show("FrmManutVendedor não está aberto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao Excluir o registro: " + erro.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FrmCadVendedor_Load(object sender, EventArgs e)
        {
            if (StatusOperacao == "ALTERAR")
            {
                return;
            }
            if (StatusOperacao == "NOVO")
            {
                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryVendedor);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                VendedorID = NovoCodigo;
                txtVendedorID.Text = numeroComZeros;
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            Utilitario.LimpaCampo(this);

            int NovoCodigo = Utilitario.GerarProximoCodigo(QueryVendedor);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
            string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
            VendedorID = NovoCodigo;
            txtVendedorID.Text = numeroComZeros;
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

                int NovoCodigo = Utilitario.GerarProximoCodigo(QueryVendedor);//RetornaCodigoContaMaisUm(QueryUsuario).ToString();
                string numeroComZeros = Utilitario.AcrescentarZerosEsquerda(NovoCodigo, 6);
                VendedorID = NovoCodigo;
                txtVendedorID.Text = numeroComZeros;

                ((FrmManutVendedor)Application.OpenForms["FrmManutVendedor"]).HabilitarTimer(true);

            }
            if (StatusOperacao == "EXCLUSÃO")
            {
                if (MessageBox.Show("Deseja Excluir? \n\n O Usuário: " + txtNomeVendedor.Text + " ??? ", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExcluirRegistro();
                }
            }
        }

        private void FrmCadVendedor_KeyDown(object sender, KeyEventArgs e)
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
