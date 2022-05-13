using System.Reflection;
using bekk.VyLogParser.ConsoleApp;
using bekk.VyLogParser.Library;

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

var result = LogReader.Execute(arguments);

Console.WriteLine($"Total log entries {result.Count}\n");

foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
{
    var count = message.Count().ToString().PadLeft(5, ' ');
    Console.WriteLine($"{count} {message.Key}");
}

Console.WriteLine();

LogHelper.Execute(arguments, result);