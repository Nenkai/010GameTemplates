// This is an extractor for DIR+FIL files
// Requires .NET 10.0 SDK - https://dotnet.microsoft.com/en-us/download/dotnet/10.0
// Usage: dotnet run <path to .DIR file>
// ------------------------------------------------------------------------------------
Console.WriteLine("-----------------------------------------");
Console.WriteLine($"- FIL/DIR extractor for Virus - The Battle Field (PS1) by Nenkai");
Console.WriteLine("-----------------------------------------");
Console.WriteLine("- https://github.com/Nenkai");
Console.WriteLine("- https://twitter.com/Nenkaai");
Console.WriteLine("-----------------------------------------");
Console.WriteLine("");

if (args.Length == 0)
{
    Console.WriteLine("Usage: <path to .DIR file>");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("File not found: " + args[0]);
    return;
}

if (!Path.GetExtension(args[0].ToUpper()).Equals(".DIR"))
{
    Console.WriteLine("Must be a .DIR file" + args[0]);
    return;
}

string packFileName = args[0];
var dirStream = File.OpenRead(packFileName);
var dirBinaryStream = new BinaryReader(dirStream);
var dataStream = File.OpenRead(Path.ChangeExtension(packFileName, ".FIL"));

Dictionary<string, FileInfo> Files = [];
int lastOffset = 0;
int numFiles = dirBinaryStream.ReadInt32();
for (int i = 0; i < numFiles; i++)
{
    byte[] fileNameBuffer = new byte[0x14];
    dirBinaryStream.ReadExactly(fileNameBuffer);
    string fileName = System.Text.Encoding.ASCII.GetString(fileNameBuffer.AsSpan(0, fileNameBuffer.AsSpan().IndexOf<byte>(0)));
    int size = dirBinaryStream.ReadInt32();

    Console.WriteLine($"{fileName} @ {lastOffset:X} (size: {size:X})");
    Files.Add(fileName, new FileInfo()
    {
        FileName = fileName,
        Offset = lastOffset,
        Length = size
    });

    lastOffset += size;
    lastOffset = (int)Align((uint)lastOffset, 0x800);
}

foreach (var file in Files)
{
    dataStream.Position = file.Value.Offset;
    byte[] fileBuffer = new byte[file.Value.Length];
    dataStream.ReadExactly(fileBuffer);

    string outputDir = Path.Combine(Path.GetDirectoryName(packFileName)!, Path.GetFileNameWithoutExtension(packFileName) + "_extracted");
    string outputFilePath = Path.Combine(outputDir, file.Value.FileName);
    Console.WriteLine("Writing: " + outputFilePath);
    Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);
    File.WriteAllBytes(outputFilePath, fileBuffer);
}

static uint Align(uint x, uint alignment)
{
    uint mask = ~(alignment - 1);
    return (x + (alignment - 1)) & mask;
}

public class FileInfo
{
    public string FileName { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
}
