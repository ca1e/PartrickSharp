namespace Decryptor
{
    internal class Encryption
    {
        const int dataSize = 0x5C000 - 0x40;
        const int endOffset = 0x5C000 - 0x30;

        public static byte[] Decrypt(byte[] course)
        {
            var endBytes = course.Skip(endOffset);
            var randBytes = endBytes.Skip(0x10).Take(0x10).ToArray();

            var encryData = course.Skip(0x10).Take(dataSize).ToArray();
            var keyBytes = RandKey.GetRandKey(randBytes);
            var ivBytes = endBytes.Take(0x10).ToArray();

            return AesUtil.Decrypt(encryData, keyBytes, ivBytes);
        }
    }
}
