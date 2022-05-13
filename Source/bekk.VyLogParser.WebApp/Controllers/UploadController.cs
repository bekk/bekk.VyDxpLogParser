using bekk.VyLogParser.Library;
using bekk.VyLogParser.Models;
using Microsoft.AspNetCore.Mvc;

namespace bekk.VyLogParser.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : Controller
{
    [HttpPost(Name = "UploadFile")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), $"Upload\\{DateTime.Now.Ticks}");
        var fileNameOnly = await FileHelper.WriteFile(file, rootFolder);
        var fileName = new FileInfo(Path.Combine(rootFolder, fileNameOnly));
        var logItems = LogReader.Execute(new Arguments
        {
            ClearOutputDirectory = false,
            OutputDirectory = fileName.Directory!,
            ZipFile = fileName
        });

        return Ok();
    }
}