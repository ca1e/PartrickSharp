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
var ivBytes = endBytes.Take(0x10).ToArray();

var decrypData = AesUtil.AESDecrypt(encryData, keyBytes, ivBytes);

File.WriteAllBytes(output, decrypData);

Console.WriteLine("Done!");
