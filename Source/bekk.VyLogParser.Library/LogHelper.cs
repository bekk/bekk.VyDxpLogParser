using System.Globalization;
using bekk.VyLogParser.Models;
using Newtonsoft.Json;

namespace bekk.VyLogParser.Library;

public static class LogHelper
{
    public static void Execute(Arguments arguments, List<LogItem> result)
    {
        WriteAllLogItemsAsJson(arguments, result);

        WriteAllLogItems(arguments, result);

        WriteSummaryLog(arguments, result);
    }

    private static void WriteSummaryLog(Arguments arguments, IEnumerable<LogItem> result)
    {
        StreamWriter? txtWriter = null;
        try
        {
            using (txtWriter = File.CreateText(Path.Combine(arguments.OutputDirectory.FullName, "Summary.log")))
            {
                foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
                {
                    var count = message.Count().ToString().PadLeft(5, ' ');
                    txtWriter.WriteLine($"{count} {message.Key}");
                }
            }
        }
        finally
        {
            txtWriter?.Close();
        }
    }

    private static void WriteAllLogItemsAsJson(Arguments arguments, List<LogItem> result)
    {
        TextWriter? jsonWriter = null;
        try
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(result, Formatting.Indented);
            jsonWriter = new StreamWriter(Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.json"));
            jsonWriter.Write(contentsToWriteToFile);
        }
        finally
        {
            jsonWriter?.Close();
        }
    }

    private static void WriteAllLogItems(Arguments arguments, List<LogItem> result)
    {
        StreamWriter? txtWriter = null;
        try
        {
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
        }
    }
}