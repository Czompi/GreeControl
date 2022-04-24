namespace GreeNativeSdk
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class Crypto
    {
        public static readonly string GenericKey = "a3K8Bx%2r8Y7#xDh";

        private static readonly byte[] CbcKey = new byte[]
        {
                17,
                /*-101*/0x9b,
                /*-16*/0xf0,
                /*-50*/0xce,

                16,
                88,
                114,
                75,

                31,
                18,
                /*-84*/0xac,
                /*-87*/0xa9,

                51,
                /*-17*/0xef,
                16,
                69
        };

        private static readonly byte[] CbcIv = new byte[]
        {
                86,
                33,
                23,
                0x99,

                109,
                9,
                61,
                40,

                0xdd,
                0xb3,
                0xba,
                105,

                90,
                46,
                111,
                88
        };

        public static string EncryptGenericData(string input)
        {
            return EncryptData(input, GenericKey);
        }

        public static string EncryptData(string input, string key)
        {
            try
            {
                var aes = CreateAes(key);
                var encryptor = aes.CreateEncryptor();
                var inputBuffer = Encoding.UTF8.GetBytes(input);
                var encrypted = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                return Convert.ToBase64String(encrypted, Base64FormattingOptions.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to encrypt data. Exception: {e}");
                return null;
            }
        }

        public static string DecryptGenericData(string input)
        {
            return DecryptData(input, GenericKey);
        }

        public static string DecryptData(string input, string key)
        {
            try
            {
                var encrypted = Convert.FromBase64String(input);
                var aes = CreateAes(key);
                var decryptor = aes.CreateDecryptor();
                var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to decrypt data. Exception: {e}");
                return null;
            }
        }

        public static string DecryptCbcData(string input)
        {
            try
            {
                var encrypted = Convert.FromBase64String(input);
                var aes = CreateAesCbc();
                var decryptor = aes.CreateDecryptor();
                var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to decrypt data. Exception: {e}");
                return null;
            }
        }

        private static Aes CreateAes(string key)
        {
            var aes = Aes.Create();

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = Encoding.ASCII.GetBytes(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            return aes;
        }

        private static Aes CreateAesCbc()
        {
            var aes = Aes.Create();

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = CbcKey;
            aes.IV = CbcIv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            return aes;
        }
    }
}
