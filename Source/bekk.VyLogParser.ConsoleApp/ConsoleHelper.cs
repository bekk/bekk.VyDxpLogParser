using bekk.VyLogParser.Models;

namespace bekk.VyLogParser.ConsoleApp;

public static class ConsoleHelper
{
    public static Arguments? ParseArguments()
    {
        var arguments = Environment.GetCommandLineArgs().ToList();

        if (arguments.Count == 1)
        {
            return null;
        }

        var rawZipPath = arguments.Skip(1).Take(1).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(rawZipPath))
        {
            return null;
        }

        var sourceFile = new FileInfo(rawZipPath);

        var destinationFolder = new DirectoryInfo(arguments.GetArgument("o") ?? sourceFile.DirectoryName!);
        var clearDestinationFolder = bool.Parse(arguments.GetArgument("c") ?? "false");

        return new Arguments
        {
            ZipFile = sourceFile,
            OutputDirectory = destinationFolder,
            ClearOutputDirectory = clearDestinationFolder
        };

    }

    public static string? GetArgument(this IEnumerable<string> args, string name)
    {
        try
        {
            var item = args.FirstOrDefault(x => x.StartsWith($"-{name}="));
            var argumentValue = item?.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            return argumentValue;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void PrintHelp()
    {
        Console.WriteLine("Missing arguments!");
        Console.WriteLine("Example: bekk.VyLogParser.ConsoleApp \"c:\\downloads\\source.zip\" [optional] -o=\"c:\\destinationFolder\" -c=true|false");

        Console.WriteLine("First argument must be the archive file");

        Console.WriteLine("[Optional] -o Output directory (Default uses source folder)");
        Console.WriteLine("[Optional] -c Clear output directory (Default is false)");
    }
}