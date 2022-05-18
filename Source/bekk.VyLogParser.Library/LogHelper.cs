using System.Globalization;
using bekk.VyLogParser.Models;
using Newtonsoft.Json;

namespace bekk.VyLogParser.Library;

public static class LogHelper
{
    public static ParseResponse Execute(Arguments arguments, List<LogItem> logItems)
    {
        var resultsAsJson = WriteAllLogItemsAsJson(arguments, logItems);
        var resultAsLogItems = WriteAllLogItems(arguments, logItems);
        var summaryLogs = logItems.GroupBy(x => x.Title);
        var resultAsSummary = WriteSummaryLog(arguments, logItems, out var summary);

        return new ParseResponse
        {
            ResultsAsJsonFile = resultsAsJson,
            ResultAsLogItemsFile = resultAsLogItems,
            ResultAsSummaryFile = resultAsSummary,
            Summary = summary,
            MinLogDate = logItems.Min(x => x.Date).ToString("dd.MM.yyyy HH:mm:ss"),
            MaxLogDate = logItems.Max(x => x.Date).ToString("dd.MM.yyyy HH:mm:ss")
        };
    }

    private static string WriteSummaryLog(Arguments arguments, IReadOnlyCollection<LogItem> logItems, out List<string> summary)
    {
        StreamWriter? txtWriter = null;
        string fileName;

        try
        {
            fileName = Path.Combine(arguments.OutputDirectory.FullName, "Summary.txt");
            summary = new List<string>();

            using (txtWriter = File.CreateText(fileName))
            {
                foreach (var message in logItems.GroupBy(x => x.Title).OrderByDescending(x => x.Count()))
                {
                    var minSummaryDate = logItems.Where(x => x.Title == message.Key).Min(x => x.Date);
                    var maxSummaryDate = logItems.Where(x => x.Title == message.Key).Max(x => x.Date);

                    var count = message.Count().ToString().PadLeft(5, ' ');
                    var text = $"{count} {message.Key}";
                    txtWriter.WriteLine(text);
                    summary.Add(text);

                    summary.Add("<br/>");

                    text = message.Count() > 1
                        ? $"First occurrence: {minSummaryDate}, last occurrence: {maxSummaryDate}"
                        : $"Occurrence: {minSummaryDate}";

                    txtWriter.WriteLine($"      {text}");
                    summary.Add($"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{text}");

                    txtWriter.WriteLine("\r\n");
                    summary.Add("<br/><br/>");
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
            fileName = Path.Combine(arguments.OutputDirectory.FullName, "AllLogItems.txt");
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