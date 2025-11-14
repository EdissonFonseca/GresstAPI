using System.Security.Cryptography;
using System.Text;

namespace Gresst.Infrastructure.Authentication
{
    public static class CryptoService
    {
        public const string ENCRYPTION_KEY = "@gr355tt0k3ng3n3r@t0r";
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes AES

        public static string Encrypt(string stringToEncrypt)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY.PadRight(32).Substring(0, 32)); // 32 bytes
                    aes.IV = IV;

                    byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncrypt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputBytes, 0, inputBytes.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string Decrypt(string stringToDecrypt)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY.PadRight(32).Substring(0, 32));
                    aes.IV = IV;

                    byte[] cipherBytes = Convert.FromBase64String(stringToDecrypt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.FlushFinalBlock();
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string EncryptOld(string stringToEncrypt)
        {
            const string OldEncryptionKey = "@rkfl0w4";
            byte[] key = { };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(OldEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string DecryptOld(string stringToDecrypt)
        {
            const string OldEncryptionKey = "@rkfl0w4";
            byte[] key = { };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(OldEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
