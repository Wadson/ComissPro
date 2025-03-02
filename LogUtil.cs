using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComissPro
{
    class LogUtil
    {
        private static readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogSistemaComissPro.txt");
        private static readonly object lockObject = new object();

        public static void Registrar(string mensagem)
        {
            lock (lockObject) // Para evitar problemas em acesso concorrente
            {
                string entradaLog = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensagem}";
                File.AppendAllText(logPath, entradaLog + Environment.NewLine);
            }
        }
    }
}
