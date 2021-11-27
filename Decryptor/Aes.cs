using System.Security.Cryptography;
using System.Text;

namespace Decryptor
{
    internal class AesUtil
    {
        public static byte[] AESDecrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;

            var plainBytes = aesAlg.DecryptCbc(cipherText, IV);
            return plainBytes;
        }
    }
}
