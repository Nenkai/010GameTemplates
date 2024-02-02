using System;
using System.IO;

Console.WriteLine("FPAC file unpacker by Nenkai");

if (args.Length < 2)
{
    Console.WriteLine("Missing arguments. <input file> <output directory> [--extract-nested]");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("Error: Input file does not exist.");
    return;
}

Directory.CreateDirectory(args[1]);

bool _extractNested = args.Length >= 3 && args[2] == "--extract-nested";
ExtractPackFile(args[0], args[1], _extractNested);


void ExtractPackFile(string pacPath, string outputFile, bool extractNested)
{
    using var fs = new FileStream(pacPath, FileMode.Open);
    using var br = new BinaryReader(fs);

    if (br.ReadUInt32() != 0x43415046)
    {
        Console.WriteLine("Error: Pack magic does not match expected.");
        return;
    }

    if (br.ReadUInt16() != 0x201)
    {
        Console.WriteLine("Error: Pack version does not match expected 0x201.");
        return;
    }

    string pacFileName = Path.GetFileName(pacPath);

    br.ReadInt16(); // IsLoaded
    uint fileCount = br.ReadUInt32();
    uint fileDescriptorsSize = br.ReadUInt32(); // Pretty much whole TOC
    Console.WriteLine($"{pacFileName}: {fileCount} files");

    // Files are ordered by their path names for BSearch
    for (int i = 0; i < fileCount; i++)
    {
        br.BaseStream.Position = 0x10 + (i * 0x10);
        int filePathOffset = br.ReadInt32();
        uint unk = br.ReadUInt32(); // Maybe checksum, executable digging only points towards it being used for caching - 0015B900 (Pack::GetFileUnkByFileIndex)
        uint fileOffset = br.ReadUInt32();
        int fileSize = br.ReadInt32();

        br.BaseStream.Position = filePathOffset;
        string filePath = ReadNullTerminated(br);

        string fullLocalPath = Path.Combine(outputFile, filePath);
        Console.WriteLine($"[{pacFileName}] -> {filePath} ({fileSize} bytes, unk: {BitConverter.ToString(BitConverter.GetBytes(unk))})");

        // Not ideal to load it all in memory but who cares
        br.BaseStream.Position = fileOffset;
        byte[] file = br.ReadBytes(fileSize);

        Directory.CreateDirectory(Path.GetDirectoryName(fullLocalPath));
        File.WriteAllBytes(fullLocalPath, file);

        if (extractNested && Path.GetExtension(filePath).Equals(".pac"))
        {
            Console.WriteLine($"Extracting nested pac file: {filePath}");
            ExtractPackFile(fullLocalPath, fullLocalPath + "_extracted", true);
        }
    }
}

static string ReadNullTerminated(BinaryReader br)
{
    string str = "";
    char ch;
    while ((int)(ch = br.ReadChar()) != 0)
        str += ch;
    return str;
}