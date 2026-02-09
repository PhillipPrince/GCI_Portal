using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Security
    {
        public async Task<string> ConfigureConn(string dbConn, string connSrv, string connDb, string connUi, string connPass)
        {
            var connectionString = string.Empty;

            try
            {
                connectionString = dbConn;

                if (connectionString.Trim().Length == 0 || string.IsNullOrEmpty(connectionString))
                {
                    var type = MethodBase.GetCurrentMethod()?.ReflectedType;
                    if (type != null)
                    {
                        Loggers.DoLogs(type.Name + "-> Connection String Missing");
                    }

                    return connectionString;
                }

                connSrv = connSrv == string.Empty ? string.Empty : await DecryptStringAES(connSrv, "GCI");
                connDb = connDb == string.Empty ? string.Empty : await DecryptStringAES(connDb, "GCI");
                connUi = connUi == string.Empty ? string.Empty : await DecryptStringAES(connUi, "GCI");
                connPass = connPass == string.Empty ? string.Empty : await DecryptStringAES(connPass, "GCI");

                // ------- Server Name
                connectionString = connectionString.Contains("[{SV}]") && connSrv != string.Empty
                    ? connectionString.Replace("[{SV}]", connSrv)
                    : string.Empty;
                // ------- Database Name
                connectionString = connectionString.Contains("[{DB}]") && connDb != string.Empty
                    ? connectionString.Replace("[{DB}]", connDb)
                    : string.Empty;
                // ------- User Name
                connectionString = connectionString.Contains("[{UI}]") && connUi != string.Empty
                    ? connectionString.Replace("[{UI}]", connUi)
                    : string.Empty;
                // ------- User password
                connectionString = connectionString.Contains("[{PW}]") && connPass != string.Empty
                    ? connectionString.Replace("[{PW}]", connPass)
                    : string.Empty;

                return await Task.FromResult(connectionString);
            }
            catch (Exception ex)
            {
                var info = MethodBase.GetCurrentMethod()?.ReflectedType;
                if (info == null)
                {
                    return await Task.FromResult(connectionString);
                }

                Loggers.DoLogs(info?.Name + "->" + ex.Message);
                return await Task.FromResult(connectionString);
            }
        }

        public string EncryptStringAES(string plainText, string password)
        {
            password = password == null || password == "" ? "GCI" : password;

            byte[] byPwd = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] byPwdHash = SHA256Managed.Create().ComputeHash(byPwd);

            byte[] byText = Encoding.UTF8.GetBytes(plainText);


            byte[] bySalt = GetRandomBytes();
            byte[] byEncrypted = new byte[bySalt.Length + byText.Length];

            // Combine Salt + Text
            for (int i = 0; i < bySalt.Length; i++)
                byEncrypted[i] = bySalt[i];
            for (int i = 0; i < byText.Length; i++)
                byEncrypted[i + bySalt.Length] = byText[i];

            byEncrypted = AES_Encrypt(byEncrypted, byPwdHash);

            string result = Convert.ToBase64String(byEncrypted);
            return result;
        }


        public async Task<string> DecryptStringAES(string cipherText, string password)
        {
            try
            {
                password = password == "" || password == "" ? "GCI" : password;

                byte[] byPwd = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                byte[] byPwdHash = SHA256Managed.Create().ComputeHash(byPwd);

                byte[] byText = Convert.FromBase64String(cipherText);

                byte[] byDecrypted = AES_Decrypt(byText, byPwdHash);

                // Remove salt
                int saltLength = 8;
                byte[] byResult = new byte[byDecrypted.Length - saltLength];
                for (int i = 0; i < byResult.Length; i++)
                    byResult[i] = byDecrypted[i + saltLength];


                string plainText = Encoding.UTF8.GetString(byResult);
                return plainText;

            }
            catch (Exception ex)
            {
                Loggers.DoLogs(ex.ToString());
                return null;
            }
        }
        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };


            using (MemoryStream ms = new MemoryStream())
            {
                using (var AES = CreateAes())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    //AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] GetRandomBytes()
        {
            int saltLength = 8;//is the minumum salt length
            byte[] salt = new byte[saltLength];
            RandomNumberGenerator.Create().GetBytes(salt);
            return salt;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {

                using (var AES = CreateAes())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);


                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
