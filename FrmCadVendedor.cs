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
                string telefoneSemMascara = Regex.Replace(txtTelefone.Text, "[^0-9]", ""); // Remove máscara
                objetoModel.Telefone = telefoneSemMascara;
                objetoModel.Comissao = double.Parse(txtPercentualComissao.Text);

                // Para depuração: verificar o valor do telefone sem máscara
                MessageBox.Show($"Telefone sem máscara: {telefoneSemMascara}");

                VendedorBLL objetoBll = new VendedorBLL();

                // Verificar duplicata exata (nome + telefone)
                if (objetoBll.VendedorExiste(objetoModel.Nome, objetoModel.Telefone))
                {
                    MessageBox.Show("Já existe um vendedor cadastrado com este Nome e Telefone!",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("REGISTRO gravado com sucesso!", "Informação!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Utilitario.LimpaCampo(this);
                ((FrmManutVendedor)Application.OpenForms["FrmManutVendedor"]).HabilitarTimer(true);
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
