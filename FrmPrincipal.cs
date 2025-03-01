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
    public partial class FrmPrincipal : KryptonForm
    {
        private string StatusOperacao = "";
        public FrmPrincipal()
        {
            InitializeComponent();
            StatusOperacao = "";
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            AtualizaBarraStatus();
        }
        private void AtualizaBarraStatus()
        {
            // Obtém o caminho do diretório de execução
            string currentPath = Path.GetDirectoryName(Application.ExecutablePath);

            // Atualiza a label de usuário na barra de status
            //string usuarioLogado = FrmLogin.UsuarioConectado;
            //string nivelAcesso = FrmLogin.NivelAcesso;
            //lblUsuarioLogadoo.Text = $"{usuarioLogado}";
            //lblTipoUsuarioo.Text = $"{nivelAcesso}";

            // Atualiza a data
            string data = DateTime.Now.ToLongDateString();
            data = data.Substring(0, 1).ToUpper() + data.Substring(1);
            lblData.Text = data;

            // Exibe informações do computador
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            var informacao = Environment.UserName;
            var nomeComputador = Environment.MachineName;

            lblEstacao.Text = nomeComputador;
            lblData.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");
            
            timerH.Tick += OnTimedEvent;
            timerH.Start();
        }
        private void AbrirFormEnPanel(object Form)
        {
            if (this.panelConteiner.Controls.Count > 0)
                this.panelConteiner.Controls.RemoveAt(0);
            Form fh = Form as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panelConteiner.Controls.Add(fh);
            this.panelConteiner.Tag = fh;
            fh.Show();
        }
        private void OnTimedEvent(object sender, EventArgs e)
        {
            lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void timerH_Tick(object sender, EventArgs e)
        {
            lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnVendedor_Click(object sender, EventArgs e)
        {
            FrmManutVendedor frm = new FrmManutVendedor(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnProduto_Click(object sender, EventArgs e)
        {
            FrmManutProduto frm = new FrmManutProduto(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void btnManutencaoEntregas_Click(object sender, EventArgs e)
        {
            FrmManutençãodeEntregaBilhetes frm = new FrmManutençãodeEntregaBilhetes(StatusOperacao);

            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void btnPrestacaoContas_Click(object sender, EventArgs e)
        {
            FrmManutencaoPrestacaoDeContas frm = new FrmManutencaoPrestacaoDeContas(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void vendedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManutVendedor frm = new FrmManutVendedor(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void produtosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManutProduto frm = new FrmManutProduto(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void btnRelatorios_Click(object sender, EventArgs e)
        {
            FrmRelatoriosComissoes frm = new FrmRelatoriosComissoes();
            AbrirFormEnPanel(frm);
        }

        private void btnFerramentas_Click(object sender, EventArgs e)
        {
            FrmFerramentas frm = new FrmFerramentas();
            AbrirFormEnPanel(frm);
        }
    }
}
