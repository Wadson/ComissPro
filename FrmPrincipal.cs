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
            // Verifica o papel de parede ao iniciar o formulário
            string caminhoSalvo = Properties.Settings.Default.CaminhoPapelParede;
            if (!string.IsNullOrEmpty(caminhoSalvo) && File.Exists(caminhoSalvo))
            {
                try
                {
                    panelConteiner.BackgroundImage = Image.FromFile(caminhoSalvo);
                    panelConteiner.BackgroundImageLayout = ImageLayout.Center;
                }
                catch
                {
                    CarregarPapelPadrao(); // Se falhar, tenta o padrão
                }
            }
            else
            {
                CarregarPapelPadrao(); // Se não houver caminho salvo, tenta o padrão
            }
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
            FrmManutencaodeEntregaBilhetes frm = new FrmManutencaodeEntregaBilhetes(StatusOperacao);

            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void btnPrestacaoContas_Click(object sender, EventArgs e)
        {
            FrmManutencaoPrestacaoDeContasConcluidas frm = new FrmManutencaoPrestacaoDeContasConcluidas(StatusOperacao);
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
            frm.ShowDialog();
        }

        private void btnFluxoCaixa_Click(object sender, EventArgs e)
        {
            FrmFluxoCaixa frm = new FrmFluxoCaixa();
            frm.ShowDialog();
        }

        private void ferramentasAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAdminUnlock frm = new FrmAdminUnlock();
            frm.ShowDialog();
        }


        private void controleDeEntregasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManutencaodeEntregaBilhetes frm = new FrmManutencaodeEntregaBilhetes(StatusOperacao);
            StatusOperacao = "NOVO";
            AbrirFormEnPanel(frm);
        }

        private void FrmPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
            // Verifique se a tecla pressionada é F5
            if (e.KeyCode == Keys.F3)
            {
                // Ação do botão
                btnManutencaoEntregas.PerformClick();

            }
        }

        private void sobreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.ShowDialog();
        }





        // Método para carregar o papel de parede padrão dos Resources

        private void CarregarPapelPadrao()
        {
            try
            {
                // Tenta carregar a imagem padrão dos Resources
                if (Properties.Resources.Papel_de_Parede_Trevo_da_Sorte_1002_566 != null) // Verifica se o recurso existe
                {
                    panelConteiner.BackgroundImage = Properties.Resources.Papel_de_Parede_Trevo_da_Sorte_1002_566;
                    panelConteiner.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    // Se não houver imagem nos Resources, usa um fundo sólido
                    panelConteiner.BackgroundImage = null;
                    panelConteiner.BackColor = Color.LightGray; // Cor padrão como fallback
                    MessageBox.Show("Nenhuma imagem padrão encontrada nos Resources. Usando fundo sólido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Caso haja erro ao acessar os Resources
                panelConteiner.BackgroundImage = null;
                panelConteiner.BackColor = Color.LightGray; // Cor padrão como fallback
                MessageBox.Show($"Erro ao carregar o papel padrão: {ex.Message}. Usando fundo sólido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void trocarPapelParedeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja trocar o papel de parede?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp|Todos os Arquivos|*.*";
                    openFileDialog.Title = "Selecione um novo papel de parede";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string caminhoImagem = openFileDialog.FileName;
                            if (File.Exists(caminhoImagem))
                            {
                                panelConteiner.BackgroundImage = Image.FromFile(caminhoImagem);
                                panelConteiner.BackgroundImageLayout = ImageLayout.Center;

                                Properties.Settings.Default.CaminhoPapelParede = caminhoImagem;
                                Properties.Settings.Default.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao carregar a imagem: {ex.Message}. Tentando papel de parede padrão.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CarregarPapelPadrao();
                        }
                    }
                }
            }
        }
    }
}
