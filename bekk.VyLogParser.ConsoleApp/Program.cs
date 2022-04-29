using System.Reflection;
using bekk.VyLogParser.ConsoleApp;
using Newtonsoft.Json;

Console.WriteLine($"Simple EPi log parser v{Assembly.GetExecutingAssembly().GetName().Version}\n");

var arguments = Environment.GetCommandLineArgs().ToList();

if (arguments.Count == 1)
{
    Console.WriteLine("Missing arguments!");
    Console.WriteLine("Example: bekk.VyLogParser.ConsoleApp \"c:\\downloads\\source.zip\" \"c:\\destinationFolder\"");
    return;
}

var zipPath = arguments.Skip(1).Take(1).FirstOrDefault();

if (string.IsNullOrWhiteSpace(zipPath))
{
    Console.WriteLine("Missing zip file argument, aborting!");
    return;
}

var extractPath = arguments.Skip(2).Take(1).FirstOrDefault() ?? @"c:\temp\Destination";

var reader = new LogReader();
var result = reader.Execute(zipPath, extractPath);

Console.WriteLine($"Total log entries {result.Count}\n");

foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
{
    var count = message.Count().ToString().PadLeft(5, ' ');
    Console.WriteLine($"{count} {message.Key}");
}

TextWriter? writer = null;
try
{
    Console.WriteLine($"Writing AllLogItems.json");
    var contentsToWriteToFile = JsonConvert.SerializeObject(result);
    writer = new StreamWriter(Path.Combine(extractPath, "AllLogItems.json"));
    writer.Write(contentsToWriteToFile);
}
finally
{
    writer?.Close();
}