using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComissPro
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (!TrialManager.IsTrialActive())
            {
                MessageBox.Show("O período de avaliação de 30 dias expirou. O sistema está bloqueado para edições.\n" +
                    "Contate o suporte para desbloqueio.", "Trial Expirado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (TrialManager.IsLicensed())
            {
                MessageBox.Show("Sistema licenciado permanentemente.", "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                int remainingDays = TrialManager.GetRemainingDays();
                MessageBox.Show($"Bem-vindo! Você tem {remainingDays} dias restantes no período de avaliação.",
                    "Trial Ativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmPrincipal());
        }
    }
}
