using System.Globalization;
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

Console.WriteLine();

TextWriter? jsonWriter = null;
try
{
    Console.WriteLine("Writing AllLogItems.json");
    var contentsToWriteToFile = JsonConvert.SerializeObject(result);
    jsonWriter = new StreamWriter(Path.Combine(extractPath, "AllLogItems.json"));
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

    using (txtWriter = File.CreateText(Path.Combine(extractPath, "AllLogItems.log")))
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