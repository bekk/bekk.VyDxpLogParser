using System.IO.Compression;

namespace bekk.VyLogParser.ConsoleApp;

internal class LogReader
{
    internal List<LogItem> Execute(Arguments arguments)
    {
        if (arguments.ClearOutputDirectory && arguments.OutputDirectory.Exists)
        {
            arguments.OutputDirectory.Delete(true);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        ZipFile.ExtractToDirectory(arguments.ZipFile.FullName, arguments.OutputDirectory.FullName, true);

        var files = Directory.GetFiles(arguments.OutputDirectory.FullName, "*.csv", SearchOption.AllDirectories);

        var logItems = new List<LogItem>();
        foreach (var file in files)
        {
            using var reader = new StreamReader(file);
            LogItem? logItem = null;

            while (!reader.EndOfStream)
            {
                var data = reader.ReadLine();
                if (data == null) continue;
                var temp = data.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (temp[0] == "date") continue;

                if (DateTime.TryParse(temp[0], out var date))
                {
                    if (logItem != null)
                    {
                        logItems.Add(logItem);
                    }

                    logItem = new LogItem
                    {
                        Date = date,
                        Level = temp[1],
                        ApplicationName = temp[2],
                        InstanceId = temp[3],
                        EventTickCount = temp[4],
                        EventId = temp[5],
                        PId = temp[6],
                        Tid = temp[7],
                        Message = temp[8].Replace("\"", string.Empty) + "\r\n",
                        Title = temp[8].Replace("\"", string.Empty)
                    };
                    continue;
                }

                if (logItem != null)
                    logItem.Message += data + "\r\n";
            }

            if (logItem != null)
            {
                logItems.Add(logItem);
            }
        }

        return logItems.OrderBy(x => x.Date).ToList();
    }
}

public class LogItem
{
    public DateTime Date { get; set; }
    public string Level { get; set; } = null!;
    public string ApplicationName { get; set; } = null!;
    public string InstanceId { get; set; } = null!;
    public string EventTickCount { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string PId { get; set; } = null!;
    public string Tid { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Title { get; set; } = null!;

}