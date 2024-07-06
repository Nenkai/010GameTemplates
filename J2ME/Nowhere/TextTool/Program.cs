using System.Text;

namespace TextTool;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Nowhere MotorolaE1000 TextTool by Nenkai");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: <to-txt/from-txt> <file_name>");
            return;
        }

        if (!File.Exists(args[1]))
        {
            Console.WriteLine($"ERROR: File '{args[1]}' does not exist");
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
        var textFile = new TextFile();
        textFile.Read(fileName);

        string outputPath = Path.ChangeExtension(fileName, ".txt");
        textFile.WriteTxt(outputPath);

        Console.WriteLine($"Done -> to-txt {outputPath} (do not edit ----- lines.)");
    }

    static void FromTxt(string fileName)
    {
        var textFile = new TextFile();
        textFile.ReadFromText(fileName);

        string outputPath = Path.ChangeExtension(fileName, "");
        textFile.WriteFile(outputPath);

        Console.WriteLine($"Done -> from-txt {outputPath}");
    }
}
