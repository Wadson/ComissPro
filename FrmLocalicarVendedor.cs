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
    public partial class FrmLocalicarVendedor : ComissPro.FrmModelo
    {
        public FrmLocalicarVendedor()
        {
            InitializeComponent();
        }

        private void FrmLocalicarVendedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SelecionarVendedor();
        }

        private void dataGridPesquisar_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridPesquisar.CurrentRow != null)
            {
                LinhaAtual = dataGridPesquisar.CurrentRow.Index;
            }
        }
    }
}
