using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HuloToys_Service.Utilities.lib
{
    public static class EncryptAES
    {
        public static string EncryptMessage(string plain_text, string key, string iv)
        {
            string cipher_text = null;
            try
            {
                var encrypt_key = Get_AESKey(ConvertBase64StringToByte(key));
                var encrypt_iv = Get_AESIV(ConvertBase64StringToByte(iv));
                if (plain_text != null && plain_text.Trim() != ""
                    && encrypt_key != null && encrypt_key.Length > 0
                    && encrypt_iv != null && encrypt_iv.Length > 0)
                {
                    cipher_text = ConvertByteToBase64String(AES_EncryptToByte(plain_text, encrypt_key, encrypt_iv));
                }
            }
            catch { }
            return cipher_text;
        }
        public static string DecryptMessage(string cipher_text, string key, string iv)
        {
            string plain_text = null;
            try
            {
                var encrypt_key = Get_AESKey(ConvertBase64StringToByte(key));
                var encrypt_iv = Get_AESIV(ConvertBase64StringToByte(iv));
                if (cipher_text != null && cipher_text.Trim() != ""
                    && encrypt_key != null && encrypt_key.Length > 0
                    && encrypt_iv != null && encrypt_iv.Length > 0)
                {
                    var cipher = ConvertBase64StringToByte(cipher_text);
                    plain_text = AES_DecryptToString(cipher, encrypt_key, encrypt_iv);
                }
            }
            catch { }
            return plain_text;
        }
        public static byte[] Get_AESKey(byte[] data)
        {

            if (data.Length >= 16 && data.Length < 24)
            {
                return data.Take(16).ToArray();
            }
            if (data.Length >= 24 && data.Length < 32)
            {
                return data.Take(24).ToArray();
            }
            if (data.Length >= 32)
            {
                return data.Take(32).ToArray();
            }

            return data;

        }
        public static byte[] Get_AESIV(byte[] data)
        {

            if (data.Length >= 16)
            {
                return data.Take(16).ToArray();
            }

            return data;

        }
        public static byte[] ConvertBase64StringToByte(string text)
        {
            try
            {
                return Convert.FromBase64String(text);
            }
            catch (Exception ex)
            {
            }
            return new byte[0];
        }
        public static string ConvertByteToBase64String(byte[] data)
        {
            try
            {
                return Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public static byte[] AES_EncryptToByte(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string AES_DecryptToString(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


        public static string FormatKey(string unformattedKey)
        {
            try
            {
                return Regex.Replace(unformattedKey.Trim(), ".{4}", "$0 ");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Base64StringToURLParam(string text)
        {
            var p = text.Replace(@"/", "-").Replace("+", "_");
            return p;
        }
        public static string URLParamToBase64String(string text)
        {
            var p = text.Replace("-", @"/").Replace("_", "+");
            return p;
        }


    }

}