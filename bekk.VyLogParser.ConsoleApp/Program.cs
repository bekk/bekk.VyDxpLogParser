using System.Globalization;
using System.Reflection;
using bekk.VyLogParser.ConsoleApp;
using Newtonsoft.Json;

var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

Console.WriteLine($"Simple EPi log parser v{version.Major}.{version.Minor}.{version.Build}\n");

var commandLineArgs = Environment.GetCommandLineArgs().ToList();

if (commandLineArgs.Count == 1)
{
    ConsoleHelper.PrintHelp();
    return;
}

var arguments = ConsoleHelper.ParseArguments();

if (arguments == null)
{
    ConsoleHelper.PrintHelp();
    return;
}

var reader = new LogReader();
var result = reader.Execute(arguments);

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
    jsonWriter = new StreamWriter(Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.json"));
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

    using (txtWriter = File.CreateText(Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.log")))
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

try
{
    Console.WriteLine("Writing summary.log");

    using (txtWriter = File.CreateText(Path.Combine(arguments.OutputDirectory.FullName, "Summary.log")))
    {
        foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
        {
            var count = message.Count().ToString().PadLeft(5, ' ');
            txtWriter.WriteLine($"{count} {message.Key}");
            txtWriter.WriteLine("\r\n\r\n");
        }
    }
}
finally
{
    txtWriter?.Close();
    Console.WriteLine(" * Done");
}
