namespace bekk.VyLogParser.ConsoleApp;

internal class Arguments
{
    public FileInfo ZipFile { get; set; } = null!;
    public DirectoryInfo OutputDirectory { get; set; } = null!;
    public bool ClearOutputDirectory { get; set; }
}