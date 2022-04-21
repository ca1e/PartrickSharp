namespace Decryptor
{
    /// <summary>
    /// thanks: https://github.com/msnmkh/AES-Server-Client-Encryption
    /// </summary>
    internal class AesUtil
    {
        private static bool Invert = true; // Used on invertable functions
        
        public static byte[] Encrypt(byte[] buf, byte[] key, byte[] iv,
             Mode mode = Mode.CBC, Padding padding = Padding.PKCS7)
        {
            if (mode == Mode.CBC)
            {
                if (iv == null || iv.Length < 16)
                    return null;
            }
            int keysize = key.Length * 8;
            if ((keysize != 128) & (keysize != 192) & (keysize != 256))
                return null;
            uint[] ek = ExpandKey(key, keysize);
            var bin = new MemoryStream(PadBuffer(buf, 16, padding));
            var bout = new MemoryStream();
            var block = new byte[16];
            int c;
            byte[,] state;
            byte[] cblock = iv;
            while ((c = bin.Read(block, 0, 16)) > 0)
            {
                switch (mode)
                {
                    case Mode.ECB:
                        state = LoadState(block);
                        EncryptBlock(state, ek, keysize);
                        bout.Write(DumpState(state), 0, c);
                        break;
                    case Mode.CBC:
                        block = XorBytes(block, cblock);
                        state = LoadState(block);
                        EncryptBlock(state, ek, keysize);
                        cblock = DumpState(state);
                        bout.Write(cblock, 0, c);
                        break;
                    default:
                        return null;
                }
            }
            return bout.ToArray();
        }

        public static byte[] Decrypt(byte[] buf, byte[] key, byte[] iv,
            Mode mode = Mode.CBC, Padding padding = Padding.PKCS7)
        {
            if (mode == Mode.CBC)
            {
                if (iv == null)
                    return null;
                if (iv.Length < 16)
                    return null;
            }
            int keysize = key.Length * 8;
            if ((keysize != 128) & (keysize != 192) & (keysize != 256))
                return null;
            uint[] ek = ExpandKey(key, keysize);
            var bin = new MemoryStream(buf);
            var bout = new MemoryStream();
            var block = new byte[16];
            int c;
            byte[,] state;
            byte[] cblock = iv;
            while ((c = bin.Read(block, 0, 16)) > 0)
            {
                switch (mode)
                {
                    case Mode.ECB:
                        state = LoadState(block);
                        DecryptBlock(state, ek, keysize);
                        block = DumpState(state);
                        bout.Write(block, 0, c);
                        break;
                    case Mode.CBC:
                        state = LoadState(block);
                        DecryptBlock(state, ek, keysize);
                        byte[] pblock = DumpState(state);
                        pblock = XorBytes(pblock, cblock);
                        cblock = (byte[])block.Clone();
                        bout.Write(pblock, 0, c);
                        break;
                    default:
                        return null;
                }
            }
            byte[] b1 = bout.ToArray();
            c = GetPadCount(b1, padding);
            byte[] b2 = new byte[b1.Length - c];
            Buffer.BlockCopy(b1, 0, b2, 0, b1.Length - c);
            return b2;
        }

        // Decrypt a block loaded into a state with expanded key.
        private static void DecryptBlock(byte[,] state, uint[] key, int keysize)
        {
            int rounds;
            switch (keysize)
            {
                case 128:
                    rounds = 10;
                    break;
                case 192:
                    rounds = 12;
                    break;
                case 256:
                    rounds = 14;
                    break;
                default:
                    return;
            }
            AddRoundKey(state, GetUIntBlock(key, rounds));
            for (int i = 1; i <= rounds; i++)
            {
                ShiftRows(state, Invert);
                SubBytes(state, Invert);
                AddRoundKey(state, GetUIntBlock(key, rounds - i));
                if (i != rounds)
                    MixColumns(state, Invert);
            }
        }

        // Returns the number of bytes padding at the end of the buffer.
        private static int GetPadCount(byte[] buf, Padding padding = Padding.PKCS7)
        {
            if (padding == Padding.NONE)
                return 0;
            int c = 0;
            bool keepgoing = true;
            for (int i = (buf.Length - 1); (i >= 0) && keepgoing; i--)
                switch (padding)
                {
                    case Padding.PKCS7:
                        if ((buf[i] == buf[buf.Length - 1]))
                            c++;
                        else keepgoing = false;
                        break;
                    case Padding.ZERO:
                        if (buf[i] == 0)
                            c++;
                        else keepgoing = false;
                        break;
                }
            switch (padding)
            {
                case Padding.PKCS7:
                    if (c > buf[buf.Length - 1])
                        return buf[buf.Length - 1];
                    if (buf[buf.Length - 1] != c)
                        return 0;
                    break;
            }
            return c;
        }

        // Expand key to a subkey for each round of en(de)cryption
        private static uint[] ExpandKey(byte[] key, int keysize = 0)
        {
            if (keysize == 0)
                keysize = key.Length * 8;
            int numWords, count, init;
            switch (keysize)
            {
                case 128:
                    numWords = 44;
                    count = 4;
                    init = 4;
                    break;
                case 192:
                    numWords = 52;
                    count = 6;
                    init = 6;
                    break;
                case 256:
                    numWords = 60;
                    count = 4;
                    init = 8;
                    break;
                default:
                    return null;
            }
            uint[] expandedKey = new uint[numWords];
            int iteration = 1;

            for (int i = 0; i < init; i++)
            {
                expandedKey[i] = GetWord(key, i * 4);
            }
            int counter = 0;
            for (int i = init; i < numWords; i += count)
            {
                uint tmp = expandedKey[i - 1];
                // 256 bit keys are a special case.
                // This is implemented as making every other pass handle the extra phase and
                // doubling the passes for 256 bit keys. Note that iteration only happens on
                // the key schedule core pass.
                if ((keysize == 256) & ((counter % 2) == 1))
                {
                    tmp = SubstituteWord(tmp);
                }
                else
                {
                    tmp = KeyScheduleCore(tmp, iteration);
                    iteration++;
                }
                counter++;
                for (int j = 0; j < count; j++)
                {
                    if ((i + j) >= numWords) // Special case for 192 bit keys
                        break;
                    tmp ^= expandedKey[i - init + j];
                    expandedKey[i + j] = tmp;
                }
            }
            return expandedKey;
        }

        // Return 32 bit uint at b[offset]
        private static uint GetWord(byte[] b, int offset = 0)
        {
            uint ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret <<= 8;
                ret |= b[i + offset];
            }
            return ret;
        }

        // Passes a 32 bit unsigned int through the Rijndael S-box
        private static uint SubstituteWord(uint word)
        {
            return (uint)(Sbox.SBOX[word & 0x000000FF] |
                (Sbox.SBOX[(word >> 8) & 0x000000FF] << 8) |
                (Sbox.SBOX[(word >> 16) & 0x000000FF] << 16) |
                (Sbox.SBOX[(word >> 24) & 0x000000FF] << 24));
        }

        // Key schedule core (http://en.wikipedia.org/wiki/Rijndael_key_schedule)
        // This operation is used as an inner loop in the key schedule, and is done in the following manner:
        // The input is a 32-bit word and at an iteration number i. The output is a 32-bit word.
        // Copy the input over to the output.
        // Use the above described rotate operation to rotate the output eight bits to the left
        // Apply Rijndael's S-box on all four individual bytes in the output word
        // On just the first (leftmost) byte of the output word, exclusive OR the byte with 2 to the power 
        // of (i-1). In other words, perform the rcon operation with i as the input, and exclusive or the 
        // rcon output with the first byte of the output word
        private static uint KeyScheduleCore(uint word, int iteration)
        {
            uint wOut = SubstituteWord(RotateByteLeft(word));
            wOut ^= (uint)(CalcRcon((byte)iteration) << 24);
            return wOut;
        }

        // Rotates the value of a 32 bit unsigned int to the left 1 byte.
        private static uint RotateByteLeft(uint x)
        {
            return ((x << 8) | (x >> 24));
        }

        // Rcon is what the Rijndael documentation calls the exponentiation of 2 to a user-specified 
        // value. Note that this operation is not performed with regular integers, but in Rijndael's 
        // finite field. (http://en.wikipedia.org/wiki/Rijndael_key_schedule)
        // Rcon(0) is 0x8d because 0x8d multiplied by 0x02 is 0x01 in the finite field.
        // (http://crypto.stackexchange.com/questions/10682/rijndael-explanation-of-rcon-on-wikipedia/10683)
        // CalcRcon is based on code by Sam Trenholme (http://www.samiam.org/key-schedule.html)
        // Typically implemented as a lookup table.
        private static byte CalcRcon(byte bin)
        {
            if (bin == 0)
                return 0x8d;
            byte b1 = 1;
            while (bin != 1)
            {
                byte b2;
                b2 = (byte)(b1 & 0x80);
                b1 <<= 1;
                if (b2 == 0x80)
                    b1 ^= 0x1b;
                bin--;
            }
            return b1;
        }

        // Pads buffer with filler using various padding styles 
        private static byte[] PadBuffer(byte[] buf, int padfrom, int padto, Padding padding = Padding.PKCS7)
        {
            if ((padto < buf.Length) | ((padto - padfrom) > 255))
                return buf;
            byte[] b = new byte[padto];
            Buffer.BlockCopy(buf, 0, b, 0, padfrom);
            for (int i = padfrom; i < padto; i++)
            {
                switch (padding)
                {
                    case Padding.PKCS7:
                        b[i] = (byte)(padto - padfrom);
                        break;
                    case Padding.ZERO:
                    case Padding.NONE:
                        b[i] = 0;
                        break;
                    default:
                        return buf;
                }
            }
            return b;
        }


        // This pads to an extra block on length % blocksize = 0 for PKCS7 (this is necessary per the standard)
        // and Zero Padding (this is implementation dependent and deliberate, but really doesn't matter as
        // no matter how many nulls are added, they will be stripped). 
        // No extra block will be added on Padding.NONE, but the last block will still be zero filled.
        public static byte[] PadBuffer(byte[] buf, int blocksize, Padding padding = Padding.PKCS7)
        {
            int extraBlock = (buf.Length % blocksize) == 0 && padding == Padding.NONE ? 0 : 1;
            return PadBuffer(buf, buf.Length, ((buf.Length / blocksize) + extraBlock) * blocksize, padding);
        }

        // This will load from a flat array into the state array starting at the 
        // block of 16 at "offset". 0 for the first block of 16, 1 for the second, 2 for the third, etc...
        private static byte[,] LoadState(byte[] buf, int offset = 0)
        {
            byte[,] state = new byte[4, 4];
            int c = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    state[j, i] = buf[c + (offset * 16)];
                    c++;
                }
            return state;
        }

        // Dump state to byte array.
        private static byte[] DumpState(byte[,] state)
        {
            byte[] b = new byte[16];
            int c = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    b[c] = state[j, i];
                    c++;
                }
            return b;
        }

        // Get column[index] of state
        private static byte[] GetColumn(byte[,] state, int index)
        {
            byte[] b = new byte[4];
            for (int i = 0; i < 4; i++)
                b[i] = state[i, index];
            return b;
        }

        // Copy b to column[index] of state
        private static void PutColumn(byte[,] state, byte[] b, int index)
        {
            for (int i = 0; i < 4; i++)
                state[i, index] = b[i];
        }

        // Encrypt a block loaded into a state with expanded key.
        private static void EncryptBlock(byte[,] state, uint[] key, int keysize)
        {
            int rounds;
            switch (keysize)
            {
                case 128:
                    rounds = 10;
                    break;
                case 192:
                    rounds = 12;
                    break;
                case 256:
                    rounds = 14;
                    break;
                default:
                    return;
            }
            AddRoundKey(state, GetUIntBlock(key));
            for (int i = 1; i <= rounds; i++)
            {
                SubBytes(state);
                ShiftRows(state);
                if (i != rounds)
                    MixColumns(state);
                AddRoundKey(state, GetUIntBlock(key, i));
            }
        }

        // Xor bytes of b1 with b2, circling through b2 as many times as necessary
        private static byte[] XorBytes(byte[] b1, byte[] b2)
        {
            byte[] rb = new byte[b1.Length];
            for (int i = 0; i < b1.Length; i++)
                rb[i] = (byte)(b1[i] ^ b2[i % b2.Length]);
            return rb;
        }

        // Returns a byte array representing the uint at "index" offset of key.
        private static byte[] GetByteBlock(uint[] key, int offset = 0)
        {
            return new byte[] {
                (byte) ((key[offset] >> 24) & 0xFF),
                (byte) ((key[offset] >> 16) & 0xFF),
                (byte) ((key[offset] >> 8) & 0xFF),
                (byte) (key[offset] & 0xFF)
            };
        }

        // Returns a uint array 4 wide starting at the index "offset" of key.
        private static uint[] GetUIntBlock(uint[] key, int offset = 0)
        {
            uint[] tmp = new uint[4];
            for (int i = 0; i < 4; i++)
                tmp[i] = key[i + (offset * 4)];
            return tmp;
        }

        // AddRoundKey xors the state by a block of key data. Inverse is itself.	
        private static void AddRoundKey(byte[,] state, uint[] key)
        {
            for (int i = 0; i < 4; i++)
            {
                var kb = GetByteBlock(key, i);
                for (int j = 0; j < 4; j++)
                    state[j, i] ^= kb[j];
            }
        }

        // Takes the state array and the sbox array. Set invert = true for inverse.
        private static void SubBytes(byte[,] state, bool invert = false)
        {
            byte[] sb;
            if (invert)
                sb = Sbox.iSBOX;
            else
                sb = Sbox.SBOX;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    state[j, i] = sb[state[j, i]];
        }

        private static void _ShiftRows(byte[,] state)
        {
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < j; i++)
                {
                    byte b = state[j, 0];
                    for (int c = 1; c < 4; c++)
                        state[j, c - 1] = state[j, c];
                    state[j, 3] = b;
                }
        }

        private static void _InvShiftRows(byte[,] state)
        {
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < j; i++)
                {
                    byte b = state[j, 3];
                    for (int c = 3; c > 0; c--)
                        state[j, c] = state[j, c - 1];
                    state[j, 0] = b;
                }
        }

        // Shift rows left n columns where n is the row's index. Set invert = true for inverse.
        private static void ShiftRows(byte[,] state, bool invert = false)
        {
            if (invert)
                _InvShiftRows(state);
            else
                _ShiftRows(state);
        }

        // Mulitplication in the Galois Field. Typically implemented with lookup tables.
        // gmul is based on code by Sam Trenholme (http://www.samiam.org/galois.html)
        private static byte gmul(byte a, byte b)
        {
            byte p = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((b & 0x01) == 0x01)
                    p ^= a;
                byte hibit = (byte)(a & 0x80);
                a <<= 1;
                if (hibit == 0x80)
                    a ^= 0x1b;
                b >>= 1;
            }
            return p;
        }

        private static void _MixColumn(byte[] r)
        {
            byte[] a = new byte[4];
            for (int i = 0; i < 4; i++)
                a[i] = r[i];
            r[0] = (byte)(gmul(a[0], 2) ^ a[3] ^ a[2] ^ gmul(a[1], 3));
            r[1] = (byte)(gmul(a[1], 2) ^ a[0] ^ a[3] ^ gmul(a[2], 3));
            r[2] = (byte)(gmul(a[2], 2) ^ a[1] ^ a[0] ^ gmul(a[3], 3));
            r[3] = (byte)(gmul(a[3], 2) ^ a[2] ^ a[1] ^ gmul(a[0], 3));
        }

        private static void _InvMixColumn(byte[] r)
        {
            byte[] a = new byte[4];
            for (int i = 0; i < 4; i++)
                a[i] = r[i];
            r[0] = (byte)(gmul(a[0], 14) ^ gmul(a[3], 9) ^ gmul(a[2], 13) ^ gmul(a[1], 11));
            r[1] = (byte)(gmul(a[1], 14) ^ gmul(a[0], 9) ^ gmul(a[3], 13) ^ gmul(a[2], 11));
            r[2] = (byte)(gmul(a[2], 14) ^ gmul(a[1], 9) ^ gmul(a[0], 13) ^ gmul(a[3], 11));
            r[3] = (byte)(gmul(a[3], 14) ^ gmul(a[2], 9) ^ gmul(a[1], 13) ^ gmul(a[0], 11));
        }

        private static void MixColumn(byte[,] state, int index, bool invert = false)
        {
            byte[] col = GetColumn(state, index);
            if (invert)
                _InvMixColumn(col);
            else
                _MixColumn(col);
            PutColumn(state, col, index);
        }

        // Each byte in the column is indivdually mixed with the other bytes in the column
        // via Galois Field addition and multiplication.
        // Set invert = true for inverse.
        private static void MixColumns(byte[,] state, bool invert = false)
        {
            for (int i = 0; i < 4; i++)
                MixColumn(state, i, invert);
        }
    }
    enum Padding
    {
        ZERO, PKCS7, NONE
    }

    enum Mode
    {
        ECB, CBC
    }

    internal class Sbox
    {
        // Standard Rijndael S-Box.
        public static byte[] SBOX = {
          /* 0     1     2     3     4     5     6     7     8     9     a     b     c     d     e     f */
    /*0*/   0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76,
    /*1*/   0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0,
    /*2*/   0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15,
    /*3*/   0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27, 0xB2, 0x75,
    /*4*/   0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84,
    /*5*/   0x53, 0xD1, 0x00, 0xED, 0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF,
    /*6*/   0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8,
    /*7*/   0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2,
    /*8*/   0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73,
    /*9*/   0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB,
    /*a*/   0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79,
    /*b*/   0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08,
    /*c*/   0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B, 0xBD, 0x8B, 0x8A,
    /*d*/   0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E,
    /*e*/   0xE1, 0xF8, 0x98, 0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF,
    /*0f*/  0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16
        };

        // Standard Rijndael inverse S-Box.
        public static byte[] iSBOX = {
           /* 0     1     2     3     4     5     6     7     8     9     a     b     c     d     e     f */
    /*0*/   0x52, 0x09, 0x6A, 0xD5, 0x30, 0x36, 0xA5, 0x38, 0xBF, 0x40, 0xA3, 0x9E, 0x81, 0xF3, 0xD7, 0xFB,
    /*1*/   0x7C, 0xE3, 0x39, 0x82, 0x9B, 0x2F, 0xFF, 0x87, 0x34, 0x8E, 0x43, 0x44, 0xC4, 0xDE, 0xE9, 0xCB,
    /*2*/   0x54, 0x7B, 0x94, 0x32, 0xA6, 0xC2, 0x23, 0x3D, 0xEE, 0x4C, 0x95, 0x0B, 0x42, 0xFA, 0xC3, 0x4E,
    /*3*/   0x08, 0x2E, 0xA1, 0x66, 0x28, 0xD9, 0x24, 0xB2, 0x76, 0x5B, 0xA2, 0x49, 0x6D, 0x8B, 0xD1, 0x25,
    /*4*/   0x72, 0xF8, 0xF6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xD4, 0xA4, 0x5C, 0xCC, 0x5D, 0x65, 0xB6, 0x92,
    /*5*/   0x6C, 0x70, 0x48, 0x50, 0xFD, 0xED, 0xB9, 0xDA, 0x5E, 0x15, 0x46, 0x57, 0xA7, 0x8D, 0x9D, 0x84,
    /*6*/   0x90, 0xD8, 0xAB, 0x00, 0x8C, 0xBC, 0xD3, 0x0A, 0xF7, 0xE4, 0x58, 0x05, 0xB8, 0xB3, 0x45, 0x06,
    /*7*/   0xD0, 0x2C, 0x1E, 0x8F, 0xCA, 0x3F, 0x0F, 0x02, 0xC1, 0xAF, 0xBD, 0x03, 0x01, 0x13, 0x8A, 0x6B,
    /*8*/   0x3A, 0x91, 0x11, 0x41, 0x4F, 0x67, 0xDC, 0xEA, 0x97, 0xF2, 0xCF, 0xCE, 0xF0, 0xB4, 0xE6, 0x73,
    /*9*/   0x96, 0xAC, 0x74, 0x22, 0xE7, 0xAD, 0x35, 0x85, 0xE2, 0xF9, 0x37, 0xE8, 0x1C, 0x75, 0xDF, 0x6E,
    /*a*/   0x47, 0xF1, 0x1A, 0x71, 0x1D, 0x29, 0xC5, 0x89, 0x6F, 0xB7, 0x62, 0x0E, 0xAA, 0x18, 0xBE, 0x1B,
    /*b*/   0xFC, 0x56, 0x3E, 0x4B, 0xC6, 0xD2, 0x79, 0x20, 0x9A, 0xDB, 0xC0, 0xFE, 0x78, 0xCD, 0x5A, 0xF4,
    /*c*/   0x1F, 0xDD, 0xA8, 0x33, 0x88, 0x07, 0xC7, 0x31, 0xB1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xEC, 0x5F,
    /*d*/   0x60, 0x51, 0x7F, 0xA9, 0x19, 0xB5, 0x4A, 0x0D, 0x2D, 0xE5, 0x7A, 0x9F, 0x93, 0xC9, 0x9C, 0xEF,
    /*e*/   0xA0, 0xE0, 0x3B, 0x4D, 0xAE, 0x2A, 0xF5, 0xB0, 0xC8, 0xEB, 0xBB, 0x3C, 0x83, 0x53, 0x99, 0x61,
    /*0f*/  0x17, 0x2B, 0x04, 0x7E, 0xBA, 0x77, 0xD6, 0x26, 0xE1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0C, 0x7D
        };
    }

    internal class SubBytes
    {
        public static void SubBytesMatrix(ref byte[,] state, bool invert = false)
        {
            byte[] sb;
            if (invert)
                sb = Sbox.iSBOX;
            else
                sb = Sbox.SBOX;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    state[j, i] = sb[state[j, i]];

        }
    }
}
