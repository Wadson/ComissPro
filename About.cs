using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace ComissPro
{
    public partial class About : KryptonForm
    {
        public About()
        {
            InitializeComponent();
        }
       
        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // O link para abrir uma conversa no WhatsApp com o número específico
            string whatsappNumber = "5594992659732"; // Substitua pelo número desejado
            string url = $"https://wa.me/{whatsappNumber}";

            // Abre o link no navegador padrão
            System.Diagnostics.Process.Start(url);
        }
    }
}
