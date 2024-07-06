using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace NowhereTool;

public class TextFile
{
    private static Encoding _textEncoding = Encoding.GetEncoding("Windows-1252");

    public List<string> Strings { get; set; } = new();
    public void Read(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        using var bs = new BinaryStream(fs, ByteConverter.Little);
        int count = bs.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            bs.BaseStream.Position = 4 + (i * sizeof(int));
            int strOffset = bs.ReadInt32();

            bs.Position = strOffset + 1;
            string str = bs.ReadString(StringCoding.ZeroTerminated, _textEncoding);
            Strings.Add(str);
        }
    }

    public void ReadFromText(string fileName)
    {
        using var reader = new StreamReader(fileName);
        reader.ReadLine();

        string last = null;
        while (!reader.EndOfStream)
        {
            string str = reader.ReadLine();
            if (str.StartsWith("--------------"))
            {
                Strings.Add(last.TrimEnd('\n'));
                last = string.Empty;
                continue;
            }
            else if (string.IsNullOrEmpty(str))
                last += '\n';
            else
                last += str + '\n';
        }

        if (!string.IsNullOrEmpty(last))
            Strings.Add(last);
    }

    public void WriteTxt(string outputFile)
    {
        using var sw = new StreamWriter(outputFile);
        for (int i = 0; i < Strings.Count; i++)
        {
            string? str = Strings[i];
            sw.WriteLine($"--------------------[{i}]--------------------");
            sw.WriteLine(str);
        }

        sw.WriteLine($"--------------------[END]--------------------");
    }

    public void WriteFile(string outputFile)
    {
        using var fs = new FileStream(outputFile, FileMode.Create);
        using var bs = new BinaryStream(fs, ByteConverter.Little);

        bs.WriteUInt32((uint)Strings.Count);

        bs.Position += (Strings.Count + 1) * 4;

        long lastPos = bs.Position;
        for (int i = 0; i < Strings.Count; i++)
        {
            bs.Position = lastPos;

            long strPos = bs.Position;
            bs.WriteByte(0);
            bs.WriteString(Strings[i], StringCoding.Raw, _textEncoding);
            bs.Align(0x04, grow: true);
            lastPos = bs.Position;

            bs.Position = 4 + (i * 0x04);
            bs.WriteUInt32((uint)strPos);

            bs.Position = lastPos;
        }

        bs.Position = sizeof(uint) + ((Strings.Count) * sizeof(uint));
        bs.WriteUInt32((uint)lastPos);
    }
}
