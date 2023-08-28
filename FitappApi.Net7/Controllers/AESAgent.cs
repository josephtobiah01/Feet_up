namespace FitappApi.Net7.Controllers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// Simple AES encrption / decryption routines
    /// </summary>
    public class AESAgent
    {
        const int SaltSize = 32;
        const int KeySize = 256;

        /// <summary>
        /// AES Encrpt
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string message, string key)
        {
            //Contract.Requires<ArgumentNullException>(message != null);
            //Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(key));
            if (message == null) { throw new ArgumentNullException("message"); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException("key"); }
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();

            using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, AESAgent.SaltSize))
            {
                byte[] saltBytes = rfc.Salt;
                byte[] keyBytes = rfc.GetBytes(32);
                byte[] ivBytes = rfc.GetBytes(16);

                using (AesManaged aes = new AesManaged() { KeySize = AESAgent.KeySize })
                {
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(keyBytes, ivBytes))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                            var streamWriter = new StreamWriter(cs);
                            streamWriter.Write(message);
                            streamWriter.Flush(); // flush streamwriter into cs
                            cs.FlushFinalBlock(); // flush cs into ms

                            byte[] cipherTextBytes = ms.ToArray();
                            Array.Resize(ref saltBytes, saltBytes.Length + cipherTextBytes.Length);
                            Array.Copy(cipherTextBytes, 0, saltBytes, AESAgent.SaltSize, cipherTextBytes.Length);

                            return Convert.ToBase64String(saltBytes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// AES decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string key)
        {
            //Contract.Requires<ArgumentNullException>(cipherText != null);
            //Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(key));
            if (cipherText == null) { throw new ArgumentNullException("message"); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException("key"); }
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();


            byte[] allBytes = Convert.FromBase64String(cipherText);
            byte[] saltBytes = allBytes.Take(AESAgent.SaltSize).ToArray();
            byte[] cipherTextBytes = allBytes.Skip(AESAgent.SaltSize).Take(allBytes.Length - AESAgent.SaltSize).ToArray();

            using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, saltBytes))
            {
                byte[] keyBytes = rfc.GetBytes(32);
                byte[] ivBytes = rfc.GetBytes(16);

                using (AesManaged aes = new AesManaged())
                {
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(keyBytes, ivBytes))
                    {
                        using (var ms = new MemoryStream(cipherTextBytes))
                        {
                            var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                            var sr = new StreamReader(cs);

                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}