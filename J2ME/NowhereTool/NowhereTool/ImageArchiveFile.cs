using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace NowhereTool;

public class ImageArchiveFile
{
    public int Unk1 { get; set; } = 1;
    public int Unk2 { get; set; } = 1;
    public int Unk3 { get; set; } = 16;
    public byte[] FileData { get; set; }
}
