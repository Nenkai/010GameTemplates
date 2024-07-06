using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace NowhereTool;

public class ImageArchive
{
    public List<ImageArchiveFile> Files { get; set; } = new();
    public void Read(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        using var bs = new BinaryStream(fs, ByteConverter.Little);
        int count = bs.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            bs.BaseStream.Position = 4 + (i * sizeof(int));
            int imgOffset = bs.ReadInt32();

            bs.Position = imgOffset;

            ImageArchiveFile file = new ImageArchiveFile();
            file.Unk1 = bs.ReadInt32();
            file.Unk2 = bs.ReadInt32();
            file.Unk3 = bs.ReadInt32();
            int fileSize = bs.ReadInt32();
            file.FileData = bs.ReadBytes(fileSize - 0x10);
            Files.Add(file);
        }
    }

    public void ReadFromFolder(string folder)
    {
        foreach (var file in Directory.GetFiles(folder)
            .OrderBy(e => int.Parse(Path.GetFileNameWithoutExtension(e))))
        {
            byte[] bytes = File.ReadAllBytes(file);
            Files.Add(new ImageArchiveFile()
            {
                FileData = bytes,
            });
        }
    }

    public void ExtractTo(string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < Files.Count; i++)
        {
            File.WriteAllBytes($"{Path.Combine(outputFolder, $"{i}.png")}", Files[i].FileData);
        }
    }

    public void WriteFile(string outputFile)
    {
        using var fs = new FileStream(outputFile, FileMode.Create);
        using var bs = new BinaryStream(fs, ByteConverter.Little);

        bs.WriteUInt32((uint)Files.Count);

        bs.Position += (Files.Count + 1) * 4;

        long lastPos = bs.Position;
        for (int i = 0; i < Files.Count; i++)
        {
            bs.Position = lastPos;

            long strPos = bs.Position;
            bs.WriteInt32(Files[i].Unk1);
            bs.WriteInt32(Files[i].Unk2);
            bs.WriteInt32(Files[i].Unk3);
            bs.WriteInt32(Files[i].FileData.Length + 0x10);
            bs.WriteBytes(Files[i].FileData);
            bs.WriteByte(0x00);
            bs.Align(0x04, grow: true);
            lastPos = bs.Position;

            bs.Position = 4 + (i * 0x04);
            bs.WriteUInt32((uint)strPos);

            bs.Position = lastPos;
        }

        bs.Position = sizeof(uint) + ((Files.Count) * sizeof(uint));
        bs.WriteUInt32((uint)lastPos);
    }
}
