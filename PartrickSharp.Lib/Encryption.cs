namespace PartrickSharp;

public class Encryption
{
    private const string course_MAGIC = "SCDL";

    public static byte[] DecryptCourse(byte[] course)
    {
        if(course.Length != 0x5C000)
        {
            throw new ArgumentException($"Invalid course data length. got {course.Length}");
        }
        using var stream = new MemoryStream(course);
        var header = new byte[0x10];
        var encrypted = new byte[0x5BFC0];
        var iv = new byte[0x10];
        var stateSeed = new byte[0x10];
        var cmac = new byte[0x10];

        /// <summary>
        /// <see cref="https://github.com/0Liam/smm2-documentation"/>
        /// </summary>
        stream.Read(header);
        // 0 on 1.0.0/1.0.1 courses, 1 on 1.0.2
        var formatVersion = BitConverter.ToUInt32(header, 0);
        // 1=Quest, 8=Network, 10=Later, 11=Save, 16=Course
        var fileType = BitConverter.ToInt16(header, 0x4);
        // Usually 0/1 for Courses
        var unKnown =  BitConverter.ToInt16(header, 0x6);
        var crc = BitConverter.ToUInt32(header, 0x8);
        var magic = System.Text.Encoding.Default.GetString(header[0xc..0x10]);
        ///
        stream.Read(encrypted);
        stream.Read(iv);
        stream.Read(stateSeed);
        stream.Read(cmac);

        if(fileType == 0x10 && magic != course_MAGIC)
        {
            throw new ArgumentException($"Invalid course magic. got {magic}");
        }

        var keyBytes = RandKey.GetRandKey(stateSeed, out var cmacKey);
        var decrypted = AesUtil.Decrypt(encrypted, keyBytes, iv);

        var crcCalced = CRC32.GetCRC32(decrypted);
        var cmacCalced = AesUtil.AESCMAC(cmacKey, decrypted);

        if(BitConverter.ToString(cmacCalced) != BitConverter.ToString(cmac))
        {
            throw new InvalidDataException($"Invalid AES-CMAC. got {BitConverter.ToString(cmacCalced)}");
        }

        if(crcCalced != crc)
        {
            throw new InvalidDataException($"Invalid CRC32. got {crcCalced}");
        }

        return decrypted;
    }

    public static byte[] EncryptCourse(byte[] raw)
    {
        var rand = new Random(Environment.TickCount);
        var iv = new byte[0x10];
        var stateSeed = new byte[0x10];
        rand.NextBytes(iv);
        rand.NextBytes(stateSeed);

        var keyBytes = RandKey.GetRandKey(stateSeed, out var cmacKey);
        var encrypted = AesUtil.Encrypt(raw, keyBytes, iv, Mode.CBC, Padding.NONE);
        
        uint crc32 = CRC32.GetCRC32(raw);
        var cmac = AesUtil.AESCMAC(cmacKey, raw);

        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes((uint)0x1));
        stream.Write(BitConverter.GetBytes((ushort)0x10)); // fileType
        stream.Write(BitConverter.GetBytes((ushort)0x0));
        stream.Write(BitConverter.GetBytes(crc32));
        stream.Write(System.Text.Encoding.Default.GetBytes(course_MAGIC)); // magic
        stream.Write(encrypted);
        stream.Write(iv);
        stream.Write(stateSeed);
        stream.Write(cmac);
        return stream.ToArray();
    }
}
