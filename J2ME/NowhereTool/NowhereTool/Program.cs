using System.Text;

namespace NowhereTool;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Nowhere MotorolaE1000 Tool by Nenkai");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: <to-txt/from-txt/to-i/extract-i> <file_name>");
            return;
        }

        try
        {

            if (args[0] == "to-txt")
            {
                ToTxt(args[1]);
            }
            else if (args[0] == "from-txt")
            {
                FromTxt(args[1]);
            }
            else if (args[0] == "to-i")
            {
                ToI(args[1]);
            }
            else if (args[0] == "extract-i")
            {
                ExtractI(args[1]);
            }
            else
            {
                Console.WriteLine("Usage: <to-txt/from-txt> <file_name>");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    static void ToTxt(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"ERROR: File '{fileName}' does not exist");
            return;
        }

        var textFile = new TextFile();
        textFile.Read(fileName);

        string outputPath = Path.ChangeExtension(fileName, ".txt");
        textFile.WriteTxt(outputPath);

        Console.WriteLine($"Done -> to-txt {outputPath} (do not edit ----- lines.)");
    }

    static void FromTxt(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"ERROR: File '{fileName}' does not exist");
            return;
        }

        var textFile = new TextFile();
        textFile.ReadFromText(fileName);

        string outputPath = Path.ChangeExtension(fileName, "");
        textFile.WriteFile(outputPath);

        Console.WriteLine($"Done -> from-txt {outputPath}");
    }

    static void ToI(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Console.WriteLine($"ERROR: File '{folder}' does not exist");
            return;
        }

        var imageArchive = new ImageArchive();
        imageArchive.ReadFromFolder(folder);

        string outputPath = Path.ChangeExtension(folder, ".i");
        imageArchive.WriteFile(outputPath);

        Console.WriteLine($"Done -> to-i {outputPath} (do not edit ----- lines.)");
    }

    static void ExtractI(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"ERROR: File '{path}' does not exist");
            return;
        }

        var imageArchive = new ImageArchive();
        imageArchive.Read(path);

        string dir = Path.GetDirectoryName(path);
        string fileName = Path.GetFileNameWithoutExtension(path);
        imageArchive.ExtractTo(Path.Combine(dir, $"{fileName}_extracted"));

        Console.WriteLine($"Done -> extract-i {dir}");
    }
}
