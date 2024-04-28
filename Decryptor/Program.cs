// See https://aka.ms/new-console-template for more information

using PartrickSharp;

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

Console.WriteLine($"Decrypting course {input}...");

var bcdFileBytes = File.ReadAllBytes(input);

var decrypData = Encryption.DecryptCourse(bcdFileBytes);
var CI = CourseSMM2.BytesToStructure<Course>(decrypData);
Console.WriteLine($"{CI}");
var encryptedData = Encryption.EncryptCourse(decrypData);

File.WriteAllBytes(output, decrypData);
File.WriteAllBytes($"{output}.ren", encryptedData);

Console.WriteLine("Done!");
