using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace ComissPro
{
    public class TrialManager
    {
        private const string RegistryPath = @"Software\ComissPro\Trial";
        private const string RegistryKeyName = "InstallDate";
        private const string RegistryLicenseKey = "LicenseStatus";
        private const int TrialDays = 30;
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("X7K9P2M4Q8J5N3L6");
        /*
             * Passos para Testar o Trial Expirado
                Aqui está como você pode configurar o sistema para simular que o trial de 30 dias já expirou:

                1. Entender o Registro
                O TrialManager grava a data de instalação em HKEY_CURRENT_USER\Software\ComissPro\Trial na chave InstallDate.
                Essa data é criptografada com AES usando a chave fixa EncryptionKey ("X7K9P2M4Q8J5N3L6") e um IV fixo (zeros).
         * */
        // Verifica se o sistema está licenciado permanentemente
        public static bool IsLicensed()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (key != null)
                {
                    string licenseStatus = (string)key.GetValue(RegistryLicenseKey);
                    return licenseStatus == "Licensed";
                }
                return false;
            }
        }

        // Define o sistema como licenciado
        public static void SetLicensed()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                key.SetValue(RegistryLicenseKey, "Licensed");
            }
        }

        // Verifica se o trial está ativo (respeita o desbloqueio permanente)
        public static bool IsTrialActive()
        {
            if (IsLicensed())
            {
                return true; // Sempre ativo se licenciado
            }

            DateTime installDate = GetOrSetInstallDate();
            DateTime expirationDate = installDate.AddDays(TrialDays);
            return DateTime.Now <= expirationDate;
        }

        public static int GetRemainingDays()
        {
            if (IsLicensed())
            {
                return -1; // Indica licença permanente (sem limite de dias)
            }

            DateTime installDate = GetOrSetInstallDate();
            DateTime expirationDate = installDate.AddDays(TrialDays);
            TimeSpan remaining = expirationDate - DateTime.Now;
            return remaining.Days > 0 ? remaining.Days : 0;
        }

        private static DateTime GetOrSetInstallDate()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true) ?? Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                string encryptedDate = (string)key.GetValue(RegistryKeyName);
                if (string.IsNullOrEmpty(encryptedDate))
                {
                    DateTime installDate = DateTime.Now;
                    key.SetValue(RegistryKeyName, EncryptDate(installDate));
                    return installDate;
                }
                else
                {
                    return DecryptDate(encryptedDate);
                }
            }
        }

        private static string EncryptDate(DateTime date)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = new byte[16];
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] dateBytes = Encoding.UTF8.GetBytes(date.ToString("yyyy-MM-dd HH:mm:ss"));
                byte[] encrypted = encryptor.TransformFinalBlock(dateBytes, 0, dateBytes.Length);
                return Convert.ToBase64String(encrypted);
            }
        }

        private static DateTime DecryptDate(string encryptedDate)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.IV = new byte[16];
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedDate);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    string dateString = Encoding.UTF8.GetString(decryptedBytes);
                    return DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", null);
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}