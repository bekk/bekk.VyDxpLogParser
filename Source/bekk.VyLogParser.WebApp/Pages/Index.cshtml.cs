using bekk.VyLogParser.Library;
using bekk.VyLogParser.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bekk.VyLogParser.WebApp.Pages;

public class IndexModel : PageModel
{
    public List<FileOnDiskModel> LogFilesOnDisk { get; set; } = null!;

    public void OnGet()
    {
        LogFilesOnDisk = FileHelper.GetLogFilesOnDisk();
    }
}