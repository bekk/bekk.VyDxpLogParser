namespace bekk.VyLogParser.Models;

public class ParseResponse
{
    public string ResultsAsJsonFile { get; set; } = null!;
    public string ResultAsLogItemsFile { get; set; } = null!;
    public string ResultAsSummaryFile { get; set; } = null!;
    public List<string> Summary { get; set; } = null!;
    public string MinLogDate { get; set; } = null!;
    public string MaxLogDate { get; set; } = null!;
}