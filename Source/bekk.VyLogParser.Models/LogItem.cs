namespace bekk.VyLogParser.Models;

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