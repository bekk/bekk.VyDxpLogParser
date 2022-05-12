using System.Globalization;
using System.Reflection;
using bekk.VyLogParser.ConsoleApp;
using Newtonsoft.Json;

var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

Console.WriteLine($"Simple EPi log parser v{version.Major}.{version.Minor}.{version.Build}\n");

var arguments = Environment.GetCommandLineArgs().ToList();

if (arguments.Count == 1)
{
    Console.WriteLine("Missing arguments!");
    Console.WriteLine("Example: bekk.VyLogParser.ConsoleApp -s=\"c:\\downloads\\source.zip\" [optional] -o=\"c:\\destinationFolder\" -c=true|false");

    Console.WriteLine("-s Source zip file");
    Console.WriteLine("[Optional] -o Output directory (Default uses source folder");
    Console.WriteLine("[Optional] -c Clear output directory (Default is false)");
    return;
}

var rawZipPath = arguments.GetArgument("s");

if (string.IsNullOrWhiteSpace(rawZipPath))
{
    Console.WriteLine("Missing zip file argument, aborting!");
    return;
}

var sourceFile = new FileInfo(rawZipPath);

var destinationFolder = arguments.GetArgument("o") ?? sourceFile.DirectoryName!;
var clearDestinationFolder = bool.Parse(arguments.GetArgument("c") ?? "false");

var reader = new LogReader();
var result = reader.Execute(sourceFile.FullName, destinationFolder, clearDestinationFolder);

Console.WriteLine($"Total log entries {result.Count}\n");

foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
{
    var count = message.Count().ToString().PadLeft(5, ' ');
    Console.WriteLine($"{count} {message.Key}");
}

Console.WriteLine();

TextWriter? jsonWriter = null;
try
{
    Console.WriteLine("Writing AllLogItems.json");
    var contentsToWriteToFile = JsonConvert.SerializeObject(result, Formatting.Indented);
    jsonWriter = new StreamWriter(Path.Combine(destinationFolder, "AllLogItems.json"));
    jsonWriter.Write(contentsToWriteToFile);
}
finally
{
    jsonWriter?.Close();
    Console.WriteLine(" * Done");
}

StreamWriter? txtWriter = null;
try
{
    Console.WriteLine("Writing AllLogItems.log");

    using (txtWriter = File.CreateText(Path.Combine(destinationFolder, "AllLogItems.log")))
    {
        foreach (var logItem in result)
        {
            txtWriter.WriteLine(logItem.Date.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            txtWriter.WriteLine(logItem.Message);
            txtWriter.WriteLine("\r\n\r\n");
        }
    }
}
finally
{
    txtWriter?.Close();
    Console.WriteLine(" * Done");
}
