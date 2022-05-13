using System.Globalization;
using bekk.VyLogParser.Models;
using Newtonsoft.Json;

namespace bekk.VyLogParser.Library;

public static class LogHelper
{
    public static ParseResponse Execute(Arguments arguments, List<LogItem> result)
    {
        var resultsAsJson = WriteAllLogItemsAsJson(arguments, result);
        var resultAsLogItems = WriteAllLogItems(arguments, result);
        var resultAsSummary = WriteSummaryLog(arguments, result, out var summary);

        return new ParseResponse
        {
            ResultsAsJsonFile = resultsAsJson,
            ResultAsLogItemsFile = resultAsLogItems,
            ResultAsSummaryFile = resultAsSummary,
            Summary = summary
        };
    }

    private static string WriteSummaryLog(Arguments arguments, IEnumerable<LogItem> result, out List<string> summary)
    {
        StreamWriter? txtWriter = null;
        string fileName;

        try
        {
            fileName = Path.Combine(arguments.OutputDirectory.FullName, "Summary.txt");
            summary = new List<string>();

            using (txtWriter = File.CreateText(fileName))
            {
                foreach (var message in result.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
                {
                    var count = message.Count().ToString().PadLeft(5, ' ');
                    var text = $"{count} {message.Key}";
                    txtWriter.WriteLine(text);
                    summary.Add(text);
                }
            }
        }
        finally
        {
            txtWriter?.Close();
        }

        return fileName;
    }

    private static string WriteAllLogItemsAsJson(Arguments arguments, List<LogItem> result)
    {
        TextWriter? jsonWriter = null;
        string fileName;

        try
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(result, Formatting.Indented);

            fileName = Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.json");
            jsonWriter = new StreamWriter(fileName);
            jsonWriter.Write(contentsToWriteToFile);
        }
        finally
        {
            jsonWriter?.Close();
        }

        return fileName;
    }

    private static string WriteAllLogItems(Arguments arguments, List<LogItem> result)
    {
        StreamWriter? txtWriter = null;
        string fileName;

        try
        {
            fileName = Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.log");
            using (txtWriter = File.CreateText(fileName))
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

        return fileName;
    }
}