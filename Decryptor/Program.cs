// See https://aka.ms/new-console-template for more information

using PartrickSharp;

string input = "/Users/ca1e/Downloads/FR8WSVJMG";
string output = "/Users/ca1e/Downloads/test";

if (new FileInfo(input).Length != 0x5C000)
{
	Console.WriteLine($"Invalid Course data size. must be 0x5C000 bytes!");
	return;
}

Console.WriteLine($"Decrypting course {input}...");

var bcdFileBytes = File.ReadAllBytes(input);

var decrypData = Encryption.DecryptCourse(bcdFileBytes);
var CI = DecodeUtil.ConverBytesToStructure<Course>(decrypData);
Console.WriteLine($"{CI.Header.StartY}. GV:{CI.Header.ClearCheckGameVer}");

Console.WriteLine($"game style: {CI.Header.GameVersion:B}");
Console.WriteLine($"Timer: {CI.Header.Timer}");
Console.WriteLine($"Theme: {CI.MainArea.Header.Theme}");

var encryptedData = Encryption.EncryptCourse(decrypData);

File.WriteAllBytes(output, decrypData);
File.WriteAllBytes($"{output}.ren", encryptedData);

Console.WriteLine("Done!");
