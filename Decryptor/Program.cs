// See https://aka.ms/new-console-template for more information

using Decryptor;

if (args.Length != 2)
{
	Console.WriteLine($"Usage: decryptor <input> <output>");
	return;
}

string input = args[0];
string output = args[1];

if (new FileInfo(input).Length != 0x5C000)
{
	Console.WriteLine($"Invalid Course data size. must be 0x5C000 bytes!");
	return;
}

var bcdFileBytes = File.ReadAllBytes(input);

var dataSize = 0x5C000 - 0x40;
var endOffset = 0x5C000 - 0x30;

var endBytes = bcdFileBytes.Skip(endOffset);
var randBytes = endBytes.Skip(0x10).Take(0x10).ToArray();

var encryData = bcdFileBytes.Skip(0x10).Take(dataSize).ToArray();
var keyBytes = RandKey.GetRandKey(randBytes);
var ivBytes = endBytes.Take(4).ToArray();
var ivRealBytes = new byte[16];
ivRealBytes[0] = ivBytes[0];
ivRealBytes[4] = ivBytes[1];
ivRealBytes[8] = ivBytes[2];
ivRealBytes[12] = ivBytes[3];
var decrypData = AesUtil.AESDecrypt(encryData, keyBytes, ivRealBytes);

File.WriteAllBytes(output, decrypData);

Console.WriteLine("Done!");
