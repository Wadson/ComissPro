using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
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
                objetoModel.Telefone = Regex.Replace(txtTelefone.Text, "[^0-9]", ""); // Remove máscara
                objetoModel.Comissao = double.Parse(txtPercentualComissao.Text);

                VendedorBLL objetoBll = new VendedorBLL();

                // Verificar duplicata com mensagem específica
                if (objetoBll.ValidarDuplicata(objetoModel.Nome, objetoModel.Telefone, out string mensagemDuplicata))
                {
                    MessageBox.Show(mensagemDuplicata, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar nomes parecidos
                string nomeParecido = objetoBll.VerificarNomeParecido(objetoModel.Nome);
                if (nomeParecido != null)
                {
                    DialogResult resposta = MessageBox.Show(
                        $"Há um registro parecido cadastrado: '{nomeParecido}'. Deseja prosseguir com o lançamento?",
                        "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resposta == DialogResult.No)
                    {
                        return;
                    }
                }

                // Salvar o registro
                objetoBll.Salvar(objetoModel);
                MessageBox.Show("REGISTRO gravado com sucesso!", "Informação!!!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);                    
                Utilitario.LimpaCampo(this);
               
                //((FrmManutVendedor)Application.OpenForms["FrmManutVendedor"]).HabilitarTimer(true);

                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutVendedor"] as FrmManutVendedor;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar o cadastro: " + ex.Message);
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
                // Remove a máscara do telefone antes de atribuir ao modelo
                objetoModel.Telefone = Regex.Replace(txtTelefone.Text, "[^0-9]", "");
                objetoModel.Comissao = double.Parse(txtPercentualComissao.Text);

                VendedorBLL objetoBll = new VendedorBLL();
                objetoBll.Alterar(objetoModel);

                MessageBox.Show("Registro Alterado com sucesso!", "Alteração!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                
                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutVendedor"] as FrmManutVendedor;
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
                Model.VendedorMODEL objetoModel = new Model.VendedorMODEL();

                objetoModel.VendedorID = Convert.ToInt32(txtVendedorID.Text);
                
                VendedorBLL objetoBll = new VendedorBLL();
                objetoBll.Excluir(objetoModel);

                MessageBox.Show("Registro excluído com sucesso!", "Exclusão!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                // Tenta acessar o formulário e atualizar diretamente
                var frmManutencao = Application.OpenForms["FrmManutVendedor"] as FrmManutVendedor;
                if (frmManutencao != null)
                {
                    frmManutencao.Listar(); // Chama Listar diretamente, sem depender do Timer
                }

                Utilitario.LimpaCampo(this);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir o registro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
